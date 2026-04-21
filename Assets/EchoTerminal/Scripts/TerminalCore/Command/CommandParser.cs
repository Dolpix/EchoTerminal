using System.Collections.Generic;
using System.Linq;
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

		if (tokenizer.TryGetParser<BoundCommand>(out var bp))
		{
			var bcp = (BoundCommandParser)bp;
			bcp.SetCommandValidator(input => Parse(input).IsMatch);
			bcp.SetCommandParser(Parse);
		}
	}

	public CommandParseResult Parse(string input)
	{
		var trimmed = input.TrimStart();

		if (string.IsNullOrEmpty(trimmed))
		{
			return CommandParseResult.UnknownCommand(new() { Raw = string.Empty });
		}

		// Command names are always a single word — extract to the first space, then
		// ask CommandNameParser directly. Using the partial-parser loop would break
		// early on any command name that is a prefix of a longer registered command.
		var spaceIdx = trimmed.IndexOf(' ');
		var commandRaw = spaceIdx >= 0 ? trimmed[..spaceIdx] : trimmed;

		_tokenizer.TryGetParser<CommandName>(out var commandNameParser);
		var commandState = commandNameParser?.ParseTokenState(commandRaw) ?? TokenState.Failed;
		if (commandState == TokenState.Partial && spaceIdx >= 0)
		{
			commandState = TokenState.Failed;
		}
		var commandToken = new Token { Raw = commandRaw, State = commandState };

		if (commandToken.State != TokenState.Completed || !_registry.TryGet(commandToken.Raw, out var entries))
		{
			return CommandParseResult.UnknownCommand(commandToken);
		}

		var argInput = spaceIdx >= 0 ? trimmed[(spaceIdx + 1)..] : string.Empty;

		List<Token> bestArgTokens = null;
		var bestCompletedCount = -1;
		var bestParamCount = 0;

		foreach (var entry in entries)
		{
			var parameters = entry.Method.GetParameters();
			var expectedTypes = parameters.Select(p => p.ParameterType).ToList();
			var argTokens = string.IsNullOrWhiteSpace(argInput)
				? new()
				: _tokenizer.Tokenize(argInput, expectedTypes);

			if (argTokens.Count == parameters.Length && argTokens.All(t => t.State == TokenState.Completed))
			{
				return CommandParseResult.Match(commandToken, entries, entry, argTokens);
			}

			var completedCount = argTokens.Count(t => t.State == TokenState.Completed);
			if (completedCount <= bestCompletedCount)
			{
				continue;
			}

			bestCompletedCount = completedCount;
			bestArgTokens = argTokens;
			bestParamCount = parameters.Length;
		}

		if (bestArgTokens == null)
		{
			return CommandParseResult.NoMatch(commandToken, entries, new());
		}

		{
			for (var i = bestParamCount; i < bestArgTokens.Count; i++)
			{
				var token = bestArgTokens[i];
				token.State = TokenState.Failed;
				bestArgTokens[i] = token;
			}
		}

		return CommandParseResult.NoMatch(commandToken, entries, bestArgTokens ?? new());
	}
}
}