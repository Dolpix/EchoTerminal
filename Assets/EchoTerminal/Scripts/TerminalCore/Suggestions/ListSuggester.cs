using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;

namespace EchoTerminal
{
[SuggestorFor(typeof(IList))]
public class ListSuggester : ISuggester
{
	private SuggestorRegistry _suggestors;

	private static string GetActiveElementPartial(string inner)
	{
		var depth = 0;
		var inQuote = false;
		int lastSplit = -1;

		for (var i = 0; i < inner.Length; i++)
		{
			char c = inner[i];
			if (c == '"')
			{
				inQuote = !inQuote;
			}
			else if (!inQuote)
			{
				switch (c)
				{
					case '[' or '(':
						depth++;
						break;
					case ']' or ')':
						depth--;
						break;
					case ',' or ' ' when depth == 0:
						lastSplit = i;
						break;
				}
			}
		}

		return inner[(lastSplit + 1)..].TrimStart();
	}

	public void Init(SuggestorRegistry suggestors)
	{
		_suggestors = suggestors;
	}

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (_suggestors == null)
		{
			return Array.Empty<string>();
		}

		string inner = partial.StartsWith("[") ? partial[1..] : partial;
		if (inner.EndsWith("]"))
		{
			return Array.Empty<string>();
		}

		Type elementType = expectedType?.IsGenericType == true
			? expectedType.GetGenericArguments().FirstOrDefault()
			: null;

		if (elementType == null || !_suggestors.TryGet(elementType, out ISuggester elementSuggester))
		{
			return Array.Empty<string>();
		}

		string activeElementPartial = GetActiveElementPartial(inner);
		string innerPrefix = inner[..^activeElementPartial.Length];

		return elementSuggester.GetSuggestions(activeElementPartial, elementType)
							   .Select(s => "[" + innerPrefix + s)
							   .ToList();
	}
}
}