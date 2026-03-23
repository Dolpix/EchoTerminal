using System;

namespace EchoTerminal.Scripts.Test
{
public class BoolParser : IParser, ITokenParser
{
	public Type TargetType => typeof(bool);

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		var end = input.IndexOf(' ');
		var token = end == -1 ? input : input.Substring(0, end);

		if (bool.TryParse(token, out var value))
		{
			result = value;
			charsConsumed = token.Length;
			return true;
		}

		result = null;
		charsConsumed = 0;
		return false;
	}

	public string TypeName => "Bool";

	public TokenState Parse(string raw, bool isFinalized)
	{
		return bool.TryParse(raw, out _) ? TokenState.Resolved : TokenState.Unresolved;
	}
}
}