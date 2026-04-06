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

			var pendingParser = FindPendingParser(raw, expectedType);

			if (pendingParser != null)
			{
				while (pos < input.Length)
				{
					pos++;
					raw = input[start..pos];
					if (pendingParser.ParseTokenState(raw) != TokenState.Pending)
					{
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

	public object ParseValue(Token token)
	{
		if (token.Type != null && _parsersByType.TryGetValue(token.Type, out var parser))
		{
			return parser.ParseValue(token.Raw);
		}

		if (token.ExpectedType != null)
		{
			foreach (var p in _parsers)
			{
				var value = p.ParseValue(token.Raw, token.ExpectedType);
				if (value != null)
				{
					return value;
				}
			}
		}

		return null;
	}

	private ITokenParser FindPendingParser(string raw, Type expectedType)
	{
		if (expectedType != null && _parsersByType.TryGetValue(expectedType, out var expected))
		{
			if (expected.ParseTokenState(raw) == TokenState.Pending)
			{
				return expected;
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Pending)
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
				if (state == TokenState.Unresolved)
				{
					state = TokenState.Invalid;
				}

				return new()
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Invalid ? null : expectedType,
					ExpectedType = expectedType
				};
			}

			foreach (var p in _parsers)
			{
				var state = p.ParseTokenState(raw, expectedType);
				if (state == TokenState.Unresolved)
				{
					continue;
				}

				return new()
				{
					Raw = raw,
					State = state,
					Type = state == TokenState.Invalid ? null : expectedType,
					ExpectedType = expectedType
				};
			}

			return new() { Raw = raw, State = TokenState.Invalid, Type = null, ExpectedType = expectedType };
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Resolved)
			{
				return new() { Raw = raw, State = TokenState.Resolved, Type = p.Type };
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Pending)
			{
				return new() { Raw = raw, State = TokenState.Pending, Type = p.Type };
			}
		}

		foreach (var p in _parsers)
		{
			if (p.ParseTokenState(raw) == TokenState.Invalid)
			{
				return new() { Raw = raw, State = TokenState.Invalid, Type = p.Type };
			}
		}

		return new() { Raw = raw, State = TokenState.Unresolved, Type = typeof(string) };
	}
}
}