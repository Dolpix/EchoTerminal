using System;
using System.Collections.Generic;

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
		foreach (var p in parsers)
		{
			_parsersByType[p.Type] = p;
		}
	}

	public List<Token> Tokenize(string input, List<Type> expectedTypes = null)
	{
		var tokens = new List<Token>();

		if (string.IsNullOrWhiteSpace(input))
		{
			return tokens;
		}

		var pos = input.Length - input.TrimStart().Length;

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

			var tokenIndex = tokens.Count;
			var expectedType = expectedTypes != null && tokenIndex < expectedTypes.Count
				? expectedTypes[tokenIndex]
				: null;

			var start = pos;
			pos++;
			var raw = input[start..pos];

			var partialParser = FindPartialParser(raw, expectedType);

			if (partialParser != null)
			{
				while (pos < input.Length)
				{
					pos++;
					raw = input[start..pos];
					var next = partialParser.ParseTokenState(raw);
					if (next == TokenState.Completed)
					{
						break;
					}

					// A space is always a word boundary — retreat past it and stop.
					// A non-space Failed is not a stop: the parser may recover on the
					// next character (e.g. "-" is a Failed int but "-1" is Completed).
					if (next == TokenState.Failed && input[pos - 1] == ' ')
					{
						pos--;
						break;
					}
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

	public object ParseValue(Token token)
	{
		if (token.Type != null && _parsersByType.TryGetValue(token.Type, out var parser))
		{
			return parser.ParseValue(token.Raw);
		}

		if (token.ExpectedType == null)
		{
			return null;
		}

		foreach (var p in _parsers)
		{
			if (!p.Type.IsAssignableFrom(token.ExpectedType))
			{
				continue;
			}

			var value = p.ParseValue(token.Raw, token.ExpectedType);
			if (value != null)
			{
				return value;
			}
		}

		return null;
	}

	private ITokenParser FindPartialParser(string raw, Type expectedType)
	{
		if (expectedType != null && _parsersByType.TryGetValue(expectedType, out var expected))
		{
			if (expected.ParseTokenState(raw) == TokenState.Partial)
			{
				return expected;
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Completed)
			{
				return null;
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Partial)
			{
				return p;
			}
		}

		return null;
	}

	private Token BuildToken(string raw, Type expectedType)
	{
		if (expectedType != null)
		{
			if (_parsersByType.TryGetValue(expectedType, out var expected))
			{
				var state = expected.ParseTokenState(raw);

				return new()
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Failed ? null : expectedType,
					ExpectedType = expectedType
				};
			}

			foreach (var p in _parsers)
			{
				if (!p.Type.IsAssignableFrom(expectedType))
				{
					continue;
				}

				var state = p.ParseTokenState(raw, expectedType);
				if (state == TokenState.Failed)
				{
					continue;
				}

				return new()
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Failed ? null : expectedType,
					ExpectedType = expectedType
				};
			}

			return new() { Raw = raw, State = TokenState.Failed, Type = null, ExpectedType = expectedType };
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Completed)
			{
				return new() { Raw = raw, State = TokenState.Completed, Type = p.Type };
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Partial)
			{
				return new() { Raw = raw, State = TokenState.Partial, Type = p.Type };
			}
		}

		return new() { Raw = raw, State = TokenState.Failed, Type = typeof(string) };
	}
}
}