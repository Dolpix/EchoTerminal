using System;
using System.Linq;
using UnityEngine;

public class Vec3IntParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(Vector3Int);
	public string HintLabel => "(x, y, z)";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Failed;
		}

		if (!raw.EndsWith(")"))
		{
			string inner = raw[1..];
			int commaIndex = inner.IndexOf(',');
			var commaCount = 0;
			while (commaIndex >= 0)
			{
				commaCount++;
				if (commaCount >= 3)
				{
					return TokenState.Failed;
				}

				string component = inner[..commaIndex].Trim();
				if (!IsValidIntComponent(component))
				{
					return TokenState.Failed;
				}

				inner = inner[(commaIndex + 1)..];
				commaIndex = inner.IndexOf(',');
			}

			return !IsValidIntComponent(inner.Trim()) ? TokenState.Failed : TokenState.Partial;
		}

		string[] parts = raw[1..^1].Split(',');
		if (parts.Length != 3)
		{
			return TokenState.Failed;
		}

		foreach (string part in parts)
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
		string[] parts = raw[1..^1].Split(',');
		return new Vector3Int(
			int.Parse(parts[0].Trim()),
			int.Parse(parts[1].Trim()),
			int.Parse(parts[2].Trim())
		);
	}

	private static bool IsValidIntComponent(string s)
	{
		if (s.Length == 0)
		{
			return true;
		}

		return int.TryParse(s, out _) || s.All(c => c == '-' || char.IsDigit(c));
	}
}