using System;
using UnityEngine;

public class QuaternionParser : ITokenParser
{
	public Type Type => typeof(Quaternion);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Unresolved;
		}

		if (!raw.EndsWith(")"))
		{
			return TokenState.Pending;
		}

		var parts = raw[1..^1].Split(',');
		if (parts.Length != 4)
		{
			return TokenState.Invalid;
		}

		foreach (var part in parts)
		{
			if (!float.TryParse(part.Trim(), out _))
			{
				return TokenState.Invalid;
			}
		}

		return TokenState.Resolved;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var parts = raw[1..^1].Split(',');
		return new Quaternion(
			float.Parse(parts[0].Trim()),
			float.Parse(parts[1].Trim()),
			float.Parse(parts[2].Trim()),
			float.Parse(parts[3].Trim())
		);
	}
}