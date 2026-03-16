using System;
using System.Collections.Generic;

namespace EchoTerminal.Scripts.Test
{
public class EnumSuggestor : ISuggestor
{
	public Type TargetType => typeof(Enum);

	public IReadOnlyList<string> GetSuggestions(Type type, string partial)
	{
		var names = Enum.GetNames(type);
		var matches = new List<string>();
		foreach (var name in names)
		{
			if (name.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
			{
				matches.Add(name);
			}
		}

		return matches;
	}
}
}