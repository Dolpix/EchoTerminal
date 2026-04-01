using System.Collections.Generic;
using System.Linq;

public static class Tokenizer
{
	public static List<Token> Tokenize(string input, IList<ITokenParser> parsers)
	{
		var tokens = new List<Token>();
		var buf = "";

		foreach (var ch in input)
		{
			if (ch == ' ' && buf.Length > 0)
			{
				var anyPending = parsers.Any(p => p.ParseTokenState(buf) == TokenState.Pending);

				if (anyPending)
				{
					buf += ch;
				}
				else
				{
					tokens.Add(Resolve(buf, parsers, finalized: true));
					buf = "";
				}
			}
			else if (ch != ' ' || buf.Length > 0)
			{
				buf += ch;
			}
		}

		if (buf.Length > 0)
		{
			tokens.Add(Resolve(buf, parsers, finalized: false));
		}

		return tokens;
	}

	private static Token Resolve(string raw, IList<ITokenParser> parsers, bool finalized)
	{
		foreach (var parser in parsers)
		{
			if (parser.ParseTokenState(raw) == TokenState.Resolved)
			{
				return new() { Raw = raw, State = TokenState.Resolved, Type = parser.Type };
			}
		}

		foreach (var parser in parsers)
		{
			if (parser.ParseTokenState(raw) != TokenState.Pending)
			{
				continue;
			}

			var state = finalized ? TokenState.Invalid : TokenState.Pending;
			return new() { Raw = raw, State = state, Type = parser.Type };
		}

		foreach (var parser in parsers)
		{
			if (parser.ParseTokenState(raw) == TokenState.Invalid)
			{
				return new() { Raw = raw, State = TokenState.Invalid, Type = parser.Type };
			}
		}

		return new() { Raw = raw, State = TokenState.Unresolved, Type = null };
	}
}