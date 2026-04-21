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
		_parsersByType = new Dictionary<Type, ITokenParser>();
		foreach (ITokenParser p in parsers)
		{
			_parsersByType[p.Type] = p;
		}
	}

	private Token BuildToken(string raw, Type expectedType)
	{
		if (expectedType != null)
		{
			if (_parsersByType.TryGetValue(expectedType, out ITokenParser expected))
			{
				TokenState state = expected.ParseTokenState(raw);

				return new Token
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Failed ? null : expectedType,
					ExpectedType = expectedType
				};
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

				return new Token
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Failed ? null : expectedType,
					ExpectedType = expectedType
				};
			}

			return new Token { Raw = raw, State = TokenState.Failed, Type = null, ExpectedType = expectedType };
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Completed)
			{
				return new Token { Raw = raw, State = TokenState.Completed, Type = p.Type };
			}
		}

		foreach (ITokenParser p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Partial)
			{
				return new Token { Raw = raw, State = TokenState.Partial, Type = p.Type };
			}
		}

		return new Token { Raw = raw, State = TokenState.Failed, Type = typeof(string) };
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

	public object ParseValue(Token token)
	{
		if (token.Type != null && _parsersByType.TryGetValue(token.Type, out ITokenParser parser))
		{
			return parser.ParseValue(token.Raw);
		}

		if (token.ExpectedType == null)
		{
			return null;
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
				while (pos < input.Length)
				{
					pos++;
					raw = input[start..pos];
					TokenState next = partialParser.ParseTokenState(raw);
					if (next == TokenState.Completed)
					{
						break;
					}

					if (next != TokenState.Failed || input[pos - 1] != ' ')
					{
						continue;
					}

					pos--;
					break;
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
}
}