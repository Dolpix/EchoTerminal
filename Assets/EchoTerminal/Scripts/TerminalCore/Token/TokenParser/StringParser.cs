using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class StringParser : ITokenParser
{
	public Type Type => typeof(string);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Failed;
		}

		if (raw[0] != '"')
		{
			return !char.IsLetter(raw[0]) ? TokenState.Failed : TokenState.Completed;
		}

		int closingQuote = raw.IndexOf('"', 1);
		if (closingQuote >= 1)
		{
			return closingQuote == raw.Length - 1 ? TokenState.Completed : TokenState.Failed;
		}

		return TokenState.Partial;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')
		{
			return raw[1..^1];
		}

		return raw;
	}
}
}