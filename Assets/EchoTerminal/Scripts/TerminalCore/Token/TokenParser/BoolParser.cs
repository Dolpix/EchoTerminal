using System;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class BoolParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(bool);
	public string HintLabel => "true | false";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Failed;
		}

		string lower = raw.ToLowerInvariant();

		if (lower is "true" or "false")
		{
			return TokenState.Completed;
		}

		if ((lower.Length < "true".Length && "true".StartsWith(lower)) ||
			(lower.Length < "false".Length && "false".StartsWith(lower)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return bool.Parse(raw);
	}
}
}