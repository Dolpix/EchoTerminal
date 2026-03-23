using System.Collections.Generic;
using System.Linq;

public class IntParser : ITokenParser
{
	public string Name => "Int";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}

public class FloatParser : ITokenParser
{
	public string Name => "Float";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (float.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}

public class TargetParser : ITokenParser
{
	private readonly HashSet<string> _known;

	public TargetParser(IEnumerable<string> knownTargets)
	{
		_known = new(knownTargets);
	}

	public string Name => "Target";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (!raw.StartsWith("@"))
		{
			return TokenState.Unresolved;
		}

		if (_known.Contains(raw))
		{
			return TokenState.Resolved;
		}

		if (!isFinalized && _known.Any(t => t.StartsWith(raw)))
		{
			return TokenState.Unresolved;
		}

		return TokenState.Invalid;
	}
}

public class CommandParser : ITokenParser
{
	private readonly HashSet<string> _registry;

	public CommandParser(IEnumerable<string> commandNames)
	{
		_registry = new(commandNames);
	}

	public string Name => "Command";

	public TokenState Parse(string raw, bool isFinalized)
	{
		return _registry.Contains(raw) ? TokenState.Resolved : TokenState.Unresolved;
	}
}

public class StringParser : ITokenParser
{
	private readonly HashSet<string> _validValues;

	public StringParser(IEnumerable<string> validValues = null)
	{
		_validValues = validValues != null ? new HashSet<string>(validValues) : null;
	}

	public string Name => "String";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (string.IsNullOrEmpty(raw) || !char.IsLetter(raw[0]))
		{
			return TokenState.Unresolved;
		}

		if (_validValues == null || _validValues.Contains(raw))
		{
			return TokenState.Resolved;
		}

		if (!isFinalized && _validValues.Any(v => v.StartsWith(raw)))
		{
			return TokenState.Unresolved;
		}

		return TokenState.Invalid;
	}
}

public class QuotedStringParser : ITokenParser
{
	public string Name => "QuotedString";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (raw.Length == 0 || raw[0] != '"')
		{
			return TokenState.Unresolved;
		}

		if (raw.Length >= 2 && raw[^1] == '"')
		{
			return TokenState.Resolved;
		}

		// Opened but not closed — am I still going?
		return isFinalized ? TokenState.Invalid : TokenState.Pending;
	}
}

public class Vec3Parser : ITokenParser
{
	public string Name => "Vec3";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Unresolved;
		}

		if (!raw.EndsWith(")"))
		{
			return isFinalized ? TokenState.Invalid : TokenState.Pending;
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