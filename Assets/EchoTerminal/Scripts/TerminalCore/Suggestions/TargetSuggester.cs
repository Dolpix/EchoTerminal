using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;

namespace EchoTerminal
{
[SuggestorFor(typeof(Target))]
public class TargetSuggester : ISuggester
{
	private readonly ITargetProvider _provider;

	public TargetSuggester(ITargetProvider provider)
	{
		_provider = provider;
	}

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		IReadOnlyList<string> targets = _provider.GetTargets();
		if (string.IsNullOrEmpty(partial))
		{
			return targets;
		}

		string normalizedPartial = partial.TrimStart('@');
		if (string.IsNullOrEmpty(normalizedPartial))
		{
			return targets;
		}

		return targets.Where(t => t.TrimStart('@').StartsWith(normalizedPartial, StringComparison.OrdinalIgnoreCase))
					  .ToList();
	}
}
}