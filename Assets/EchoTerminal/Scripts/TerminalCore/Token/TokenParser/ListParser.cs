using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;

public class ListParser : ITokenParser
{
	private readonly List<ITokenParser> _parsers;
	private Tokenizer _tokenizer;

	public ListParser(List<ITokenParser> parsers)
	{
		_parsers = parsers;
	}

	public Type Type => typeof(IList);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '[')
		{
			return TokenState.Failed;
		}

		if (!IsBalanced(raw))
		{
			return TokenState.Partial;
		}

		var inner = raw[1..^1];
		if (string.IsNullOrWhiteSpace(inner))
		{
			return TokenState.Completed;
		}

		var tokens = GetTokenizer().Tokenize(Normalize(inner));

		if (tokens.Any(t => t.State != TokenState.Completed))
		{
			return TokenState.Failed;
		}

		var elementType = GetElementType(expectedType);
		if (elementType == null)
		{
			return TokenState.Completed;
		}

		{
			var elementParser = FindElementParser(elementType, tokens[0].Raw);
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
		var inner = raw[1..^1];
		if (string.IsNullOrWhiteSpace(inner))
		{
			return CreateEmptyList(expectedType);
		}

		var tokens = GetTokenizer().Tokenize(Normalize(inner));
		var elementType = GetElementType(expectedType);

		if (expectedType == null && tokens.Count > 0)
		{
			var firstType = tokens[0].Type;
			if (firstType != null && tokens.All(t => t.Type == firstType))
			{
				expectedType = typeof(List<>).MakeGenericType(firstType);
			}
		}

		var list = CreateEmptyList(expectedType);
		elementType = GetElementType(expectedType);

		foreach (var token in tokens)
		{
			var parser = FindElementParser(elementType ?? token.Type, token.Raw);
			list.Add(parser?.ParseValue(token.Raw, elementType));
		}

		return list;
	}

	private ITokenParser FindElementParser(Type elementType, string sampleRaw)
	{
		if (elementType == null)
		{
			return null;
		}

		return _parsers.FirstOrDefault(p => p.Type == elementType) ??
			   _parsers.FirstOrDefault(p => p.ParseTokenState(sampleRaw, elementType) == TokenState.Completed);
	}

	private Tokenizer GetTokenizer()
	{
		return _tokenizer ??= new(_parsers);
	}

	private static string Normalize(string inner)
	{
		var depth = 0;
		var inQuote = false;
		var chars = inner.ToCharArray();
		for (var i = 0; i < chars.Length; i++)
		{
			var c = chars[i];
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
		var elementType = GetElementType(listType) ?? typeof(object);
		return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
	}

	private static bool IsBalanced(string raw)
	{
		var depth = 0;
		var inQuote = false;
		foreach (var c in raw)
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