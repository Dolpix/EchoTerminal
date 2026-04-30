using System;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class IntParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(int);
	public string HintLabel => "0";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Completed;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return int.Parse(raw);
	}
}
}