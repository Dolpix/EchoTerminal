using System.Collections.Generic;
using System.Linq;

public class StringParser : ITokenParser
{
	private readonly HashSet<string> _validValues;

	public StringParser() { }

	public StringParser(IEnumerable<string> validValues)
	{
		_validValues = new HashSet<string>(validValues);
	}

	public System.Type Type => typeof(string);

	public TokenState Parse(string raw)
	{
		if (raw.Length == 0)
		{
			return TokenState.Unresolved;
		}

		if (raw[0] == '"')
		{
			if (raw.Length >= 2 && raw[^1] == '"')
			{
				return TokenState.Resolved;
			}

			return TokenState.Pending;
		}

		if (!char.IsLetter(raw[0]))
		{
			return TokenState.Unresolved;
		}

		if (_validValues == null || _validValues.Contains(raw))
		{
			return TokenState.Resolved;
		}

		if (_validValues.Any(v => v.StartsWith(raw)))
		{
			return TokenState.Unresolved;
		}

		return TokenState.Invalid;
	}
}