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
	}

	public CommandParseResult Parse(string input)
	{
		var tokens = _tokenizer.Tokenize(input);

		if (tokens.Count == 0)
		{
			return CommandParseResult.UnknownCommand(new() { Raw = string.Empty });
		}

		var commandToken = tokens[0];

		if (commandToken.State != TokenState.Completed || !_registry.TryGet(commandToken.Raw, out var entries))
		{
			return CommandParseResult.UnknownCommand(commandToken);
		}

		var argInput = input.TrimStart();
		var spaceAfterCommand = argInput.IndexOf(' ');
		argInput = spaceAfterCommand >= 0 ? argInput[(spaceAfterCommand + 1)..] : string.Empty;

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
		}

		return CommandParseResult.NoMatch(commandToken, entries);
	}
}
}