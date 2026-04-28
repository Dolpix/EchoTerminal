using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoTerminal.TerminalCore;

namespace EchoTerminal
{
public class CommandParser
{
	private readonly CommandRegistry _registry;
	private readonly Tokenizer _tokenizer;

	public CommandParser(CommandRegistry registry, Tokenizer tokenizer)
	{
		_registry = registry;
		_tokenizer = tokenizer;

		if (!tokenizer.TryGetParser<BoundCommand>(out ITokenParser bp))
		{
			return;
		}

		var bcp = (BoundCommandParser)bp;
		bcp.SetCommandValidator(input => Parse(input).IsMatch);
		bcp.SetCommandParser(Parse);
	}

	public CommandParseResult Parse(string input)
	{
		string trimmed = input.TrimStart();

		if (string.IsNullOrEmpty(trimmed))
		{
			return CommandParseResult.UnknownCommand(new() { Raw = string.Empty });
		}

		int spaceIdx = trimmed.IndexOf(' ');
		string commandRaw = spaceIdx >= 0 ? trimmed[..spaceIdx] : trimmed;

		_tokenizer.TryGetParser<CommandName>(out ITokenParser commandNameParser);
		TokenState commandState = commandNameParser?.ParseTokenState(commandRaw) ?? TokenState.Failed;
		if (commandState == TokenState.Partial && spaceIdx >= 0)
		{
			commandState = TokenState.Failed;
		}

		var commandToken = new Token { Raw = commandRaw, State = commandState, ExpectedType = typeof(CommandName) };

		if (commandToken.State != TokenState.Completed ||
			!_registry.TryGet(commandToken.Raw, out List<CommandEntry> entries))
		{
			return CommandParseResult.UnknownCommand(commandToken);
		}

		string argInput = spaceIdx >= 0 ? trimmed[(spaceIdx + 1)..] : string.Empty;

		List<Token> bestArgTokens = null;
		int bestCompletedCount = -1;
		int bestPartialCount = -1;
		var bestParamCount = 0;

		foreach (CommandEntry entry in entries)
		{
			ParameterInfo[] parameters = entry.Method.GetParameters();
			IEnumerable<Type> paramTypes = parameters
				.Where(p => p.GetCustomAttribute<InjectAttribute>() == null)
				.Select(p => p.ParameterType);
			List<Type> expectedTypes = entry.HasTarget
				? paramTypes.Prepend(typeof(Target)).ToList()
				: paramTypes.ToList();

			List<Token> argTokens = string.IsNullOrWhiteSpace(argInput)
				? new()
				: _tokenizer.Tokenize(argInput, expectedTypes);

			int injectedCount = parameters.Count(p => p.GetCustomAttribute<InjectAttribute>() != null);
			int paramCount = entry.HasTarget ? parameters.Length - injectedCount + 1 : parameters.Length - injectedCount;

			if (argTokens.Count == paramCount && argTokens.All(t => t.State == TokenState.Completed))
			{
				return CommandParseResult.Match(commandToken, entries, entry, argTokens);
			}

			int completedCount = argTokens.Count(t => t.State == TokenState.Completed);
			int partialCount = argTokens.Count(t => t.State == TokenState.Partial);

			bool better = completedCount > bestCompletedCount ||
			              (completedCount == bestCompletedCount && partialCount > bestPartialCount);
			if (!better)
			{
				continue;
			}

			bestCompletedCount = completedCount;
			bestPartialCount = partialCount;
			bestArgTokens = argTokens;
			bestParamCount = paramCount;
		}

		if (bestArgTokens == null)
		{
			return CommandParseResult.NoMatch(commandToken, entries, new());
		}

		{
			for (int i = bestParamCount; i < bestArgTokens.Count; i++)
			{
				Token token = bestArgTokens[i];
				token.State = TokenState.Failed;
				bestArgTokens[i] = token;
			}
		}

		return CommandParseResult.NoMatch(commandToken, entries, bestArgTokens ?? new());
	}
}
}