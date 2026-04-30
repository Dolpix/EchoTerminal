using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Structs;
using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
[SuggestorFor(typeof(BoundCommand))]
public class BoundCommandSuggester : ISuggester
{
	private CommandRegistry _commandRegistry;
	private SuggestorRegistry _suggestors;
	private CommandParser _commandParser;

	public void Init(CommandRegistry commandRegistry, SuggestorRegistry suggestors, CommandParser commandParser)
	{
		_commandRegistry = commandRegistry;
		_suggestors = suggestors;
		_commandParser = commandParser;
	}

	public IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null)
	{
		if (_commandRegistry == null)
		{
			return Array.Empty<string>();
		}

		string inner = partial.StartsWith(">") ? partial[1..] : partial;
		if (inner.EndsWith("<"))
		{
			return Array.Empty<string>();
		}

		string activePartial = GetLastSpacePartial(inner);
		string innerPrefix = inner[..^activePartial.Length];

		if (!inner.Contains(' '))
		{
			if (!_suggestors.TryGet(typeof(CommandName), out ISuggester commandSuggester))
			{
				return Array.Empty<string>();
			}

			return commandSuggester.GetSuggestions(activePartial)
								   .Select(s => ">" + innerPrefix + s)
								   .ToList();
		}

		CommandParseResult innerResult = _commandParser.Parse(inner);
		if (innerResult.Entries == null || innerResult.Entries.Count == 0)
		{
			return Array.Empty<string>();
		}

		Token.Token? innerActiveToken = GetActiveToken(innerResult);
		int paramIndex = innerActiveToken == null
			? innerResult.ArgTokens?.Count ?? 0
			: GetActiveParamIndex(innerResult);

		ISuggester paramSuggester = null;
		Type paramExpectedType = null;

		if (paramIndex >= 0)
		{
			CommandEntry bestEntry = innerResult.Entries[0];
			ParameterInfo[] parameters = bestEntry.Method.GetParameters();
			if (paramIndex < parameters.Length)
			{
				var attr = parameters[paramIndex].GetCustomAttribute<SuggestAttribute>();
				if (attr != null)
				{
					paramExpectedType = parameters[paramIndex].ParameterType;
					paramSuggester = attr.Suggester;
				}
				else if (innerActiveToken == null)
				{
					paramExpectedType = parameters[paramIndex].ParameterType;
					_suggestors.TryGet(paramExpectedType, out paramSuggester);
				}
			}
		}

		if (paramSuggester == null && innerActiveToken != null)
		{
			paramExpectedType = innerActiveToken.Value.ExpectedType;
			_suggestors.TryGet(paramExpectedType, out paramSuggester);
		}

		if (paramSuggester == null)
		{
			return Array.Empty<string>();
		}

		bool innerIsComplex = paramExpectedType != null &&
							  (paramExpectedType == typeof(BoundCommand) ||
							   typeof(IList).IsAssignableFrom(paramExpectedType));

		string suggestionInput;
		string prefixForWrap;

		if (innerIsComplex && innerActiveToken != null)
		{
			suggestionInput = innerActiveToken.Value.Raw;
			prefixForWrap = inner[..^suggestionInput.Length];
		}
		else
		{
			suggestionInput = activePartial;
			prefixForWrap = innerPrefix;
		}

		return paramSuggester.GetSuggestions(suggestionInput, paramExpectedType)
							 .Select(s => ">" + prefixForWrap + s)
							 .ToList();
	}

	private static string GetLastSpacePartial(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}

		int idx = s.LastIndexOf(' ');
		return idx < 0 ? s : s[(idx + 1)..];
	}

	private static Token.Token? GetActiveToken(CommandParseResult result)
	{
		if (result.ArgTokens == null || result.ArgTokens.Count == 0)
		{
			return null;
		}

		Token.Token last = result.ArgTokens[^1];
		return last.State != TokenState.Completed ? last : null;
	}

	private static int GetActiveParamIndex(CommandParseResult result)
	{
		if (result.ArgTokens == null)
		{
			return -1;
		}

		int last = result.ArgTokens.Count - 1;
		if (last < 0)
		{
			return -1;
		}

		return result.ArgTokens[last].State != TokenState.Completed ? last : -1;
	}
}
}