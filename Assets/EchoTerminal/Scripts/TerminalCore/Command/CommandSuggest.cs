using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoTerminal
{
public class CommandSuggest
{
	private readonly CommandParser _parser;

	public CommandSuggest(CommandParser parser)
	{
		_parser = parser;
	}

	public SuggestionContext GetSuggestions(string input)
	{
		var result = _parser.Parse(input);

		if (string.IsNullOrEmpty(result.CommandName))
		{
			return SuggestionContext.Empty;
		}

		if (result.Args == null)
		{
			var matches = new List<string>();
			foreach (var name in _parser.Registry.GetCommandNames())
			{
				if (name.StartsWith(result.CommandName, StringComparison.OrdinalIgnoreCase))
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
				ReplaceStart = result.LeadingSpaces,
				ReplaceEnd = input.Length
			};
		}

		if (result.Args.StartsWith("@") && !result.Args[1..].Contains(' '))
		{
			var hasTarget = false;
			foreach (var overload in result.Overloads)
			{
				if (overload.Params.Count > 0 && overload.Params[0].Expected.IsTarget)
				{
					hasTarget = true;
					break;
				}
			}

			if (!hasTarget)
			{
				return SuggestionContext.Empty;
			}

			var partial = result.Args[1..];
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

			var atPos = input.IndexOf('@', result.LeadingSpaces + result.CommandName.Length);
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