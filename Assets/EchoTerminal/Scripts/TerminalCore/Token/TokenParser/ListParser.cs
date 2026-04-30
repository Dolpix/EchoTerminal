using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class ListParser : ITokenParser, IRecursiveParser, IHintLabeler
{
	public Type Type => typeof(IList);
	public string HintLabel => "[a, b, c]";
	private readonly List<ITokenParser> _parsers;
	private Tokenizer _tokenizer;

	public ListParser(List<ITokenParser> parsers)
	{
		_parsers = parsers;
	}

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '[')
		{
			return TokenState.Failed;
		}

		if (!IsBalanced(raw))
		{
			string partial = raw[1..];
			List<Token> allTokens = GetAllPartialTokens(partial);

			Type type = GetElementType(expectedType);
			if (type == null)
			{
				if (allTokens == null)
				{
					return TokenState.Partial;
				}

				IEnumerable<Token> committed = allTokens.Take(allTokens.Count - 1);
				Token last = allTokens[^1];
				if (committed.Any(t => t.State == TokenState.Failed) ||
					last.State == TokenState.Failed)
				{
					return TokenState.Failed;
				}

				return TokenState.Partial;
			}

			if (allTokens is not { Count: > 0 })
			{
				return TokenState.Partial;
			}

			ITokenParser elementParser = FindElementParser(type, allTokens[0].Raw);
			List<Token> completedTokens = allTokens.Take(allTokens.Count - 1).ToList();
			Token lastToken = allTokens[^1];
			if (elementParser == null ||
				completedTokens.Any(t => elementParser.ParseTokenState(t.Raw, type) != TokenState.Completed) ||
				elementParser.ParseTokenState(lastToken.Raw, type) == TokenState.Failed)
			{
				return TokenState.Failed;
			}

			return TokenState.Partial;
		}

		string inner = raw[1..^1];
		if (string.IsNullOrWhiteSpace(inner))
		{
			return TokenState.Completed;
		}

		List<Token> tokens = GetTokenizer().Tokenize(Normalize(inner));

		if (tokens.Any(t => t.State != TokenState.Completed))
		{
			return TokenState.Failed;
		}

		Type elementType = GetElementType(expectedType);
		if (elementType == null)
		{
			return TokenState.Completed;
		}

		{
			ITokenParser elementParser = FindElementParser(elementType, tokens[0].Raw);
			if (elementParser == null ||
				tokens.Any(t => elementParser.ParseTokenState(t.Raw, elementType) != TokenState.Completed))
			{
				return TokenState.Failed;
			}
		}

		return TokenState.Completed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		string inner = raw[1..^1];
		if (string.IsNullOrWhiteSpace(inner))
		{
			return CreateEmptyList(expectedType);
		}

		List<Token> tokens = GetTokenizer().Tokenize(Normalize(inner));
		if (expectedType == null && tokens.Count > 0)
		{
			Type firstType = tokens[0].ExpectedType;
			if (firstType != null && tokens.All(t => t.ExpectedType == firstType))
			{
				expectedType = typeof(List<>).MakeGenericType(firstType);
			}
		}

		IList list = CreateEmptyList(expectedType);
		Type elementType = GetElementType(expectedType);

		foreach (Token token in tokens)
		{
			ITokenParser parser = FindElementParser(elementType ?? token.ExpectedType, token.Raw);
			list.Add(parser?.ParseValue(token.Raw, elementType));
		}

		return list;
	}

	public List<Token> GetSubTokens(string raw, Type expectedType)
	{
		TokenState outerState = ParseTokenState(raw, expectedType);
		var result = new List<Token>();

		if (raw.Length == 0 || raw[0] != '[')
		{
			result.Add(new() { Raw = raw, State = outerState, ExpectedType = expectedType });
			return result;
		}

		bool hasClose = IsBalanced(raw) && raw[^1] == ']';
		string inner = hasClose ? raw[1..^1] : raw[1..];

		TokenState bracketState = hasClose ? outerState : TokenState.Partial;
		result.Add(new() { Raw = "[", State = bracketState, ExpectedType = expectedType });

		if (inner.Length > 0)
		{
			Type elementType = GetElementType(expectedType);
			List<Token> elementTokens = hasClose
				? GetTokenizer().Tokenize(Normalize(inner))
				: GetAllPartialTokens(inner);

			if (elementTokens == null || elementTokens.Count == 0)
			{
				result.Add(new() { Raw = inner, State = bracketState, ExpectedType = expectedType });
			}
			else
			{
				ITokenParser ep = elementType != null ? FindElementParser(elementType, elementTokens[0].Raw) : null;
				var pos = 0;

				for (var i = 0; i < elementTokens.Count; i++)
				{
					string elemRaw = elementTokens[i].Raw;
					int elemStart = inner.IndexOf(elemRaw, pos, StringComparison.Ordinal);
					if (elemStart < 0)
					{
						break;
					}

					if (elemStart > pos)
					{
						result.Add(new()
						{
							Raw = inner[pos..elemStart],
							State = bracketState,
							ExpectedType = expectedType
						});
					}

					TokenState elemState = ep?.ParseTokenState(elemRaw, elementType) ?? elementTokens[i].State;

					result.Add(new() { Raw = elemRaw, State = elemState, ExpectedType = elementType });
					pos = elemStart + elemRaw.Length;
				}

				if (pos < inner.Length)
				{
					result.Add(new() { Raw = inner[pos..], State = bracketState, ExpectedType = expectedType });
				}
			}
		}

		if (hasClose)
		{
			result.Add(new() { Raw = "]", State = outerState, ExpectedType = expectedType });
		}

		return result;
	}

	private ITokenParser FindElementParser(Type elementType, string sampleRaw)
	{
		if (elementType == null)
		{
			return null;
		}

		return _parsers.FirstOrDefault(p => p.Type == elementType) ??
			   _parsers.FirstOrDefault(p => p.Type.IsAssignableFrom(elementType) &&
											p.ParseTokenState(sampleRaw, elementType) == TokenState.Completed);
	}

	private List<Token> GetAllPartialTokens(string partialInner)
	{
		var depth = 0;
		var inQuote = false;
		int lastComma = -1;
		var segments = new List<string>();

		for (var i = 0; i < partialInner.Length; i++)
		{
			char c = partialInner[i];
			if (c == '"')
			{
				inQuote = !inQuote;
			}
			else if (!inQuote)
			{
				switch (c)
				{
					case '[' or '(':
						depth++;
						break;
					case ']' or ')':
						depth--;
						break;
					case ',' when depth == 0:
						segments.Add(partialInner[(lastComma + 1)..i].Trim());
						lastComma = i;
						break;
				}
			}
		}

		string trailing = partialInner[(lastComma + 1)..].Trim();
		if (!string.IsNullOrEmpty(trailing))
		{
			segments.Add(trailing);
		}

		if (segments.Count == 0)
		{
			return null;
		}

		string inner = string.Join(" ", segments);
		return GetTokenizer().Tokenize(inner);
	}

	private Tokenizer GetTokenizer()
	{
		return _tokenizer ??= new(_parsers);
	}

	private static string Normalize(string inner)
	{
		var depth = 0;
		var inQuote = false;
		char[] chars = inner.ToCharArray();
		for (var i = 0; i < chars.Length; i++)
		{
			char c = chars[i];
			if (c == '"')
			{
				inQuote = !inQuote;
			}
			else
			{
				switch (inQuote)
				{
					case false when c is '[' or '(':
						depth++;
						break;
					case false when c is ']' or ')':
						depth--;
						break;
					case false when c == ',' && depth == 0:
						chars[i] = ' ';
						break;
				}
			}
		}

		return new(chars);
	}

	private static Type GetElementType(Type listType)
	{
		return listType?.IsGenericType == true ? listType.GetGenericArguments().FirstOrDefault() : null;
	}

	private static IList CreateEmptyList(Type listType)
	{
		Type elementType = GetElementType(listType) ?? typeof(object);
		return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
	}

	private static bool IsBalanced(string raw)
	{
		var depth = 0;
		var inQuote = false;
		foreach (char c in raw)
		{
			if (c == '"')
			{
				inQuote = !inQuote;
			}
			else
			{
				switch (inQuote)
				{
					case false when c == '[':
						depth++;
						break;
					case false when c == ']':
					{
						depth--;
						if (depth < 0)
						{
							return false;
						}

						break;
					}
				}
			}
		}

		return depth == 0;
	}
}
}