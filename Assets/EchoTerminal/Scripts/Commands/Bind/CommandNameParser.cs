using System;

namespace EchoTerminal
{
public class CommandNameParser : ITokenParser
{
	public Type TargetType => typeof(CommandName);
	public string TypeName => "CommandName";

	public System.Type Type => TargetType;

	public TokenState Parse(string raw)
	{
		if (raw.Length == 0 || raw[0] != '>')
		{
			return TokenState.Unresolved;
		}

		var close = raw.IndexOf('<', 1);
		if (close > 0)
		{
			return TokenState.Resolved;
		}

		return TokenState.Pending;
	}

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		result = null;
		charsConsumed = 0;

		if (input.Length == 0 || input[0] != '>')
		{
			return false;
		}

		var close = input.IndexOf('<', 1);

		if (close == -1)
		{
			return false;
		}

		result = new CommandName(input.Substring(1, close - 1));
		charsConsumed = close + 1;
		return true;
	}
}
}
