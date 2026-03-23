using System;
using EchoTerminal;

namespace EchoTerminal.Scripts.Test
{
public class IntParser : IParser, ITokenParser
{
	public Type TargetType => typeof(int);
	public string TypeName => "Int";

	public TokenState Parse(string raw, bool isFinalized)
	{
		return int.TryParse(raw, out _) ? TokenState.Resolved : TokenState.Unresolved;
	}

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		var end = input.IndexOf(' ');
		var token = end == -1 ? input : input.Substring(0, end);

		if (int.TryParse(token, out var value))
		{
			result = value;
			charsConsumed = token.Length;
			return true;
		}

		result = null;
		charsConsumed = 0;
		return false;
	}
}
}