using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EchoTerminal.Scripts.TerminalCore.Structs;

namespace EchoTerminal.Scripts.TerminalCore.Command
{
public readonly struct CommandParseResult
{
	public readonly bool IsMatch;
	public readonly Token.Token CommandToken;
	public readonly CommandEntry? Entry;
	public readonly List<Token.Token> ArgTokens;
	public readonly List<CommandEntry> Entries;

	private CommandParseResult(
		Token.Token commandToken,
		List<CommandEntry> entries,
		CommandEntry? entry,
		List<Token.Token> argTokens)
	{
		IsMatch = entry.HasValue;
		CommandToken = commandToken;
		Entry = entry;
		ArgTokens = argTokens;
		Entries = entries;
	}

	public static CommandParseResult UnknownCommand(Token.Token commandToken)
	{
		return new(commandToken, null, null, null);
	}

	public static CommandParseResult NoMatch(Token.Token commandToken, List<CommandEntry> entries, List<Token.Token> argTokens)
	{
		return new(commandToken, entries, null, argTokens);
	}

	public static CommandParseResult Match(
		Token.Token commandToken,
		List<CommandEntry> entries,
		CommandEntry entry,
		List<Token.Token> argTokens)
	{
		return new(commandToken, entries, entry, argTokens);
	}

	public string GetError()
	{
		if (Entries == null)
		{
			return $"Unknown command '{CommandToken.Raw}'.";
		}

		if (Entries.Count == 1)
		{
			return $"Invalid arguments for '{CommandToken.Raw}'. Expected: {BuildSignatureHint(Entries[0])}";
		}

		string signatures = string.Join(" | ", Entries.Select(BuildSignatureHint));
		return $"Invalid arguments for '{CommandToken.Raw}'. Expected one of: {signatures}";
	}

	private static string BuildSignatureHint(CommandEntry entry)
	{
		ParameterInfo[] parameters = entry.Method.GetParameters();
		if (parameters.Length == 0)
		{
			return "(no arguments)";
		}

		string paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
		return $"({paramList})";
	}
}
}