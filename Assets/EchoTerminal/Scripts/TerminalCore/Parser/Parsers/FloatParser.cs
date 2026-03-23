using System;
using System.Globalization;
using EchoTerminal;

namespace EchoTerminal.Scripts.Test
{
public class FloatParser : IParser, ITokenParser
{
	public Type TargetType => typeof(float);
	public string TypeName => "Float";

	public TokenState Parse(string raw, bool isFinalized)
	{
		return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
			? TokenState.Resolved
			: TokenState.Unresolved;
	}

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		var end = input.IndexOf(' ');
		var token = end == -1 ? input : input.Substring(0, end);

		if (float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
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