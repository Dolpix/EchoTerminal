using System.Linq;
using EchoTerminal.TerminalCore;
using UnityEngine;

namespace EchoTerminal
{
public class CommandExecutor
{
	private readonly CommandParser _commandParser;
	private readonly CommandRegistry _registry;
	private readonly Tokenizer _tokenizer;

	public CommandExecutor(CommandRegistry registry, Tokenizer tokenizer)
	{
		_registry = registry;
		_tokenizer = tokenizer;
		_commandParser = new(registry, tokenizer);
	}

	public void Execute(string input)
	{
		var result = _commandParser.Parse(input);

		if (!result.IsMatch)
		{
			Debug.LogError(BuildError(result));
			return;
		}

		if (result.Entry == null)
		{
			return;
		}

		var entry = result.Entry.Value;
		var args = result.ArgTokens.Select(t => _tokenizer.ParseValue(t)).ToArray();

		if (entry.IsStatic)
		{
			entry.Method.Invoke(null, args);
			return;
		}

		var instances = _registry.GetInstances(entry.MonoType);
		if (instances.Length == 0)
		{
			Debug.LogError($"No active instance of '{entry.MonoType.Name}' found in the scene.");
			return;
		}

		foreach (var instance in instances)
		{
			entry.Method.Invoke(instance, args);
		}
	}

	public bool TryValidateCommand(string input, out string error)
	{
		var result = _commandParser.Parse(input);

		if (result.IsMatch)
		{
			error = null;
			return true;
		}

		error = BuildError(result);
		return false;
	}

	private static string BuildError(CommandParseResult result)
	{
		if (result.Entries == null)
		{
			return $"Unknown command '{result.CommandToken.Raw}'.";
		}

		if (result.Entries.Count == 1)
		{
			return
				$"Invalid arguments for '{result.CommandToken.Raw}'. Expected: {BuildSignatureHint(result.Entries[0])}";
		}

		var signatures = string.Join(" | ", result.Entries.Select(BuildSignatureHint));
		return $"Invalid arguments for '{result.CommandToken.Raw}'. Expected one of: {signatures}";
	}

	private static string BuildSignatureHint(CommandEntry entry)
	{
		var parameters = entry.Method.GetParameters();
		if (parameters.Length == 0)
		{
			return "(no arguments)";
		}

		var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
		return $"({paramList})";
	}
}
}