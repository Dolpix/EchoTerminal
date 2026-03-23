using System;

namespace EchoTerminal.Scripts.Test
{
public class StringParser : IParser, ITokenParser
{
	public Type TargetType => typeof(string);

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		if (input.Length == 0)
		{
			result = null;
			charsConsumed = 0;
			return false;
		}

		if (input[0] == '"')
		{
			var closing = input.IndexOf('"', 1);
			if (closing != -1)
			{
				result = input.Substring(1, closing - 1);
				charsConsumed = closing + 1;
				return true;
			}
		}

		var end = input.IndexOf(' ');
		var token = end == -1 ? input : input.Substring(0, end);
		result = token;
		charsConsumed = token.Length;
		return true;
	}

	public string TypeName => "String";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (string.IsNullOrEmpty(raw))
		{
			return TokenState.Unresolved;
		}

		if (raw[0] == '"')
		{
			if (raw.Length >= 2 && raw[^1] == '"')
			{
				return TokenState.Resolved;
			}

			return isFinalized ? TokenState.Invalid : TokenState.Pending;
		}

		return TokenState.Resolved;
	}
}
}