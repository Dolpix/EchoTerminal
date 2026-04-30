using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using UnityEngine;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
[SuggestorFor(typeof(Color))]
public class ColorSuggester : ISuggester
{
	private static readonly string[] NamedColors =
	{
		"red", "green", "blue", "white", "black",
		"yellow", "cyan", "magenta", "clear", "grey", "gray"
	};

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return NamedColors;
		}

		return NamedColors
			   .Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
						   !string.Equals(c, partial, StringComparison.OrdinalIgnoreCase))
			   .ToList();
	}
}
}