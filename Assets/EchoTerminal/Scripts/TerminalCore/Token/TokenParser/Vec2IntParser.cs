using System;
using UnityEngine;

public class Vec2IntParser : ITokenParser
{
	public Type Type => typeof(Vector2Int);

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
		if (parts.Length != 2)
		{
			return TokenState.Failed;
		}

		foreach (var part in parts)
		{
			if (!int.TryParse(part.Trim(), out _))
			{
				return TokenState.Failed;
			}
		}

		return TokenState.Completed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var parts = raw[1..^1].Split(',');
		return new Vector2Int(
			int.Parse(parts[0].Trim()),
			int.Parse(parts[1].Trim())
		);
	}
}