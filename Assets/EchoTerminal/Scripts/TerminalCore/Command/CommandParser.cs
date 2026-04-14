using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;

namespace EchoTerminal
{
public readonly struct CommandParseResult
{
	public readonly bool IsMatch;
	public readonly Token CommandToken;
	public readonly CommandEntry? Entry;
	public readonly List<Token> ArgTokens;
	public readonly List<CommandEntry> Entries;

	private CommandParseResult(
		Token commandToken,
		List<CommandEntry> entries,
		CommandEntry? entry,
		List<Token> argTokens)
	{
		IsMatch = entry.HasValue;
		CommandToken = commandToken;
		Entry = entry;
		ArgTokens = argTokens;
		Entries = entries;
	}

	public static CommandParseResult UnknownCommand(Token commandToken)
	{
		return new(commandToken, null, null, null);
	}

	public static CommandParseResult NoMatch(Token commandToken, List<CommandEntry> entries)
	{
		return new(commandToken, entries, null, null);
	}

	public static CommandParseResult Match(
		Token commandToken,
		List<CommandEntry> entries,
		CommandEntry entry,
		List<Token> argTokens)
	{
		return new(commandToken, entries, entry, argTokens);
	}
}

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