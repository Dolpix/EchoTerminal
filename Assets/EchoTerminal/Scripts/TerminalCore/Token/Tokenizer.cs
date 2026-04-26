using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.TerminalCore
{
public class Tokenizer
{
	private readonly List<ITokenParser> _parsers;
	private readonly Dictionary<Type, ITokenParser> _parsersByType;

	public Tokenizer(List<ITokenParser> parsers)
	{
		_parsers = parsers;
		_parsersByType = new();
		foreach (ITokenParser p in parsers)
		{
			_parsersByType[p.Type] = p;
		}
	}

	public object ParseValue(Token token)
	{
		if (token.ExpectedType == null)
		{
			return null;
		}

		if (_parsersByType.TryGetValue(token.ExpectedType, out ITokenParser parser))
		{
			return parser.ParseValue(token.Raw, token.ExpectedType);
		}

		foreach (ITokenParser p in _parsers)
		{
			if (!p.Type.IsAssignableFrom(token.ExpectedType))
			{
				continue;
			}

			object value = p.ParseValue(token.Raw, token.ExpectedType);
			if (value != null)
			{
				return value;
			}
		}

		return null;
	}

	public List<Token> Tokenize(string input, List<Type> expectedTypes = null)
	{
		var tokens = new List<Token>();

		if (string.IsNullOrWhiteSpace(input))
		{
			return tokens;
		}

		int pos = input.Length - input.TrimStart().Length;

		while (pos < input.Length)
		{
			while (pos < input.Length && input[pos] == ' ')
			{
				pos++;
			}

			if (pos >= input.Length)
			{
				break;
			}

			int tokenIndex = tokens.Count;
			Type expectedType = expectedTypes != null && tokenIndex < expectedTypes.Count
				? expectedTypes[tokenIndex]
				: null;

			int start = pos;
			pos++;
			string raw = input[start..pos];

			ITokenParser partialParser = FindPartialParser(raw, expectedType);

			if (partialParser != null)
			{
				int lastCompletedPos = -1;

				while (pos < input.Length)
				{
					pos++;
					raw = input[start..pos];
					TokenState next = partialParser.ParseTokenState(raw);

					if (next == TokenState.Completed)
					{
						lastCompletedPos = pos;
						continue;
					}

					if (next != TokenState.Failed || input[pos - 1] != ' ')
					{
						continue;
					}

					pos = lastCompletedPos >= 0 ? lastCompletedPos : pos - 1;
					break;
				}

				raw = input[start..pos];
				if (lastCompletedPos >= 0 && partialParser.ParseTokenState(raw) == TokenState.Failed)
				{
					pos = lastCompletedPos;
				}
			}
			else
			{
				while (pos < input.Length && input[pos] != ' ')
				{
					pos++;
				}
			}

			raw = input[start..pos];
			tokens.Add(BuildToken(raw, expectedType));
		}

		return tokens;
	}

	public bool TryGetParser<T>(out ITokenParser parser)
	{
		return _parsersByType.TryGetValue(typeof(T), out parser);
	}

	public bool TryGetParser(Type type, out ITokenParser parser)
	{
		if (_parsersByType.TryGetValue(type, out parser))
		{
			return true;
		}

		foreach (ITokenParser p in _parsers.Where(p => p.Type.IsAssignableFrom(type)))
		{
			parser = p;
			return true;
		}

		return false;
	}

	private Token BuildToken(string raw, Type expectedType)
	{
		if (expectedType != null)
		{
			if (_parsersByType.TryGetValue(expectedType, out ITokenParser expected))
			{
				TokenState state = expected.ParseTokenState(raw);
				return new() { Raw = raw, State = state, ExpectedType = expectedType };
			}

			foreach (ITokenParser p in _parsers)
			{
				if (!p.Type.IsAssignableFrom(expectedType))
				{
					continue;
				}

				TokenState state = p.ParseTokenState(raw, expectedType);
				if (state == TokenState.Failed)
				{
					continue;
				}

				return new() { Raw = raw, State = state, ExpectedType = expectedType };
			}

			return new() { Raw = raw, State = TokenState.Failed, ExpectedType = expectedType };
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Completed)
			{
				return new() { Raw = raw, State = TokenState.Completed, ExpectedType = p.Type };
			}
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Partial)
			{
				return new() { Raw = raw, State = TokenState.Partial, ExpectedType = p.Type };
			}
		}

		return new() { Raw = raw, State = TokenState.Failed, ExpectedType = null };
	}

	private ITokenParser FindPartialParser(string raw, Type expectedType)
	{
		if (expectedType != null && _parsersByType.TryGetValue(expectedType, out ITokenParser expected))
		{
			if (expected.ParseTokenState(raw) == TokenState.Partial)
			{
				return expected;
			}
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Completed)
			{
				return null;
			}
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Partial)
			{
				return p;
			}
		}

		return null;
	}
}
}