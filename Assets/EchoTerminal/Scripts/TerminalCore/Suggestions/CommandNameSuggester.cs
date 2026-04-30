using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
[SuggestorFor(typeof(CommandName))]
public class CommandNameSuggester : ISuggester
{
	private readonly CommandRegistry _registry;

	public CommandNameSuggester(CommandRegistry registry)
	{
		_registry = registry;
	}

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return _registry.GetCommandNames().OrderBy(n => n).ToList();
		}

		return _registry.GetCommandNames()
						.Where(n => n.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
						.OrderBy(n => n)
						.ToList();
	}
}
}