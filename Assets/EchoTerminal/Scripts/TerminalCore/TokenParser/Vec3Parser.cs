public class Vec3Parser : ITokenParser
{
	public System.Type Type => typeof(UnityEngine.Vector3);

	public TokenState Parse(string raw)
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
		if (parts.Length != 3)
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
}