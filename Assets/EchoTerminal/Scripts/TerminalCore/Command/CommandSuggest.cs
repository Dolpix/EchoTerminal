using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoTerminal
{
public class CommandSuggest
{
	private readonly CommandRegistry _registry;

	public CommandSuggest(CommandRegistry registry)
	{
		_registry = registry;
	}

	public SuggestionContext GetSuggestions(string input)
	{
		if (!CommandProcessor.TryParseInput(input, out var commandName, out var args, out var leadingSpaces))
		{
			return SuggestionContext.Empty;
		}

		if (args == null)
		{
			var matches = new List<string>();
			foreach (var name in _registry.GetCommandNames())
			{
				if (name.StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
				{
					matches.Add(name);
				}
			}

			if (matches.Count == 0)
			{
				return SuggestionContext.Empty;
			}

			return new()
			{
				Suggestions = matches,
				ReplaceStart = leadingSpaces,
				ReplaceEnd = input.Length
			};
		}

		if (args.StartsWith("@") && !args[1..].Contains(' '))
		{
			var partial = args[1..];
			var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
			var goMatches = new List<string>();
			foreach (var go in allGOs)
			{
				if (go.name.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
				{
					goMatches.Add(go.name);
				}
			}

			if (goMatches.Count == 0)
			{
				return SuggestionContext.Empty;
			}

			var atPos = input.IndexOf('@', leadingSpaces + commandName.Length);
			return new()
			{
				Suggestions = goMatches,
				ReplaceStart = atPos + 1,
				ReplaceEnd = input.Length
			};
		}

		return SuggestionContext.Empty;
	}


}
}