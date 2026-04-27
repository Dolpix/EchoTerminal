using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal;
using EchoTerminal.TerminalCore;
using UnityEngine;

[SuggestorFor(typeof(Color))]
public class ColorSuggester : ISuggester
{
	private static readonly string[] _namedColors =
	{
		"red", "green", "blue", "white", "black",
		"yellow", "cyan", "magenta", "clear", "grey", "gray"
	};

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (string.IsNullOrEmpty(partial))
		{
			return _namedColors;
		}

		return _namedColors
			   .Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase) &&
						   !string.Equals(c, partial, StringComparison.OrdinalIgnoreCase))
			   .ToList();
	}
}