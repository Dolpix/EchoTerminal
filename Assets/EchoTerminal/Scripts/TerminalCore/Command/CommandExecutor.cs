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

	public CommandExecutor(CommandParser commandParser, CommandRegistry registry, Tokenizer tokenizer)
	{
		_registry = registry;
		_tokenizer = tokenizer;
		_commandParser = commandParser;
	}

	public void Execute(string input)
	{
		var result = _commandParser.Parse(input);

		if (!result.IsMatch)
		{
			Debug.LogError(result.GetError());
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
}
}