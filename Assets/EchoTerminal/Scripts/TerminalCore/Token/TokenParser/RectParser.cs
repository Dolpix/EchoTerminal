using System;
using UnityEngine;

public class RectParser : ITokenParser
{
	public Type Type => typeof(Rect);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Failed;
		}

		if (!raw.EndsWith(")"))
		{
			return TokenState.Partial;
		}

		var parts = raw[1..^1].Split(',');
		if (parts.Length != 4)
		{
			return TokenState.Failed;
		}

		foreach (var part in parts)
		{
			if (!float.TryParse(part.Trim(), out _))
			{
				return TokenState.Failed;
			}
		}

		return TokenState.Completed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var parts = raw[1..^1].Split(',');
		return new Rect(
			float.Parse(parts[0].Trim()),
			float.Parse(parts[1].Trim()),
			float.Parse(parts[2].Trim()),
			float.Parse(parts[3].Trim())
		);
	}
}