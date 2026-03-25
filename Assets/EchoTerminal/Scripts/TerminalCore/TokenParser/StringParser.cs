using System.Collections.Generic;
using System.Linq;

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