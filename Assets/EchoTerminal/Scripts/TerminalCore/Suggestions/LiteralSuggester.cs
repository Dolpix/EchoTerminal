using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
public class LiteralSuggester : ISuggester
{
	private readonly string[] _values;

	public LiteralSuggester(string[] values)
	{
		_values = values;
	}

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return _values;
		}

		return _values
			   .Where(v => v.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
						   !string.Equals(v, partial, StringComparison.OrdinalIgnoreCase))
			   .ToList();
	}
}
}