using System;
using System.Globalization;
using UnityEngine;

namespace EchoTerminal.Scripts.Test
{
public class Vector2Parser : IParser
{
	public Type TargetType => typeof(Vector2);

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		result = null;
		charsConsumed = 0;

		string token;
		if (input.Length > 0 && input[0] == '(')
		{
			var close = input.IndexOf(')');
			if (close == -1) return false;
			token = input.Substring(0, close + 1);
		}
		else
		{
			var end = input.IndexOf(' ');
			token = end == -1 ? input : input.Substring(0, end);
		}

		var inner = token.Length >= 2 && token[0] == '(' && token[token.Length - 1] == ')'
			? token.Substring(1, token.Length - 2)
			: token;

		var parts = inner.Split(',');
		if (parts.Length != 2)
		{
			return false;
		}

		if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x) ||
			!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
		{
			return false;
		}

		result = new Vector2(x, y);
		charsConsumed = token.Length;
		return true;
	}
}
}