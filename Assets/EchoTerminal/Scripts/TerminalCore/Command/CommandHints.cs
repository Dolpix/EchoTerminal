using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EchoTerminal
{
public class CommandHints
{
	private readonly CommandRegistry _registry;

	public CommandHints(CommandRegistry registry)
	{
		_registry = registry;
	}

	public List<string> GetHints(string input)
	{
		if (!CommandProcessor.TryParseInput(input, out var commandName, out var args, out _))
		{
			return null;
		}

		if (args == null)
		{
			return null;
		}

		if (!_registry.TryGet(commandName, out var entries))
		{
			return null;
		}

		var hints = new List<string>();

		foreach (var entry in entries)
		{
			var parameters = entry.Method.GetParameters();
			var parts = parameters.Select(p => $"<{p.Name}:{p.ParameterType.Name}>").ToList();
			
			if (!entry.IsStatic)
			{
				parts.Add("<@gameObject>");
			}

			if (parts.Count > 0)
			{
				hints.Add(string.Join(" ", parts));
			}
		}

		return hints.Count > 0 ? hints : null;
	}
}
}