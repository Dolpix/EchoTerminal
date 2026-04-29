using System;
using System.Collections.Generic;
using System.Linq;

public class StringParser : ITokenParser
{
	public Type Type => typeof(string);
	private readonly HashSet<string> _validValues;

	public StringParser()
	{
	}

	public StringParser(IEnumerable<string> validValues)
	{
		_validValues = new(validValues);
	}

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Failed;
		}

		if (raw[0] == '"')
		{
			int closingQuote = raw.IndexOf('"', 1);
			if (closingQuote >= 1)
			{
				return closingQuote == raw.Length - 1 ? TokenState.Completed : TokenState.Failed;
			}

			return TokenState.Partial;
		}

		if (!char.IsLetter(raw[0]))
		{
			return TokenState.Failed;
		}

		if (_validValues == null || _validValues.Contains(raw))
		{
			return TokenState.Completed;
		}

		if (_validValues.Any(v => v.StartsWith(raw)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')
		{
			return raw[1..^1];
		}

		return raw;
	}
}