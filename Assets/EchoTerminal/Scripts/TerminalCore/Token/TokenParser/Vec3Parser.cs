public class Vec3Parser : ITokenParser, IHintLabeler
{
	public System.Type Type => typeof(UnityEngine.Vector3);
	public string HintLabel => "(x, y, z)";

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Failed;
		}

		if (!raw.EndsWith(")"))
		{
			var inner = raw[1..];
			var commaIndex = inner.IndexOf(',');
			while (commaIndex >= 0)
			{
				var component = inner[..commaIndex].Trim();
				if (!IsValidFloatComponent(component))
				{
					return TokenState.Failed;
				}

				inner = inner[(commaIndex + 1)..];
				commaIndex = inner.IndexOf(',');
			}

			if (!IsValidFloatComponent(inner.Trim()))
			{
				return TokenState.Failed;
			}

			return TokenState.Partial;
		}

		var parts = raw[1..^1].Split(',');
		if (parts.Length != 3)
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

	private static bool IsValidFloatComponent(string s)
	{
		if (s.Length == 0)
		{
			return true;
		}

		if (float.TryParse(s, out _))
		{
			return true;
		}

		foreach (var c in s)
		{
			if (c != '-' && c != '+' && c != '.' && !char.IsDigit(c))
			{
				return false;
			}
		}

		return true;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		var parts = raw[1..^1].Split(',');
		return new UnityEngine.Vector3(
			float.Parse(parts[0].Trim()),
			float.Parse(parts[1].Trim()),
			float.Parse(parts[2].Trim())
		);
	}
}