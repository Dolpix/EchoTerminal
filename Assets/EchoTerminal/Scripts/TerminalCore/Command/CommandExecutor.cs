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
		CommandParseResult result = _commandParser.Parse(input);

		if (!result.IsMatch)
		{
			Debug.LogError(result.GetError());
			return;
		}

		if (result.Entry == null)
		{
			return;
		}

		CommandEntry entry = result.Entry.Value;
		object[] allArgs = result.ArgTokens.Select(t => _tokenizer.ParseValue(t)).ToArray();

		Target? target = null;
		object[] args = allArgs;

		if (entry.HasTarget)
		{
			if (allArgs.Length > 0 && allArgs[0] is Target t)
			{
				target = t;
				args = allArgs[1..];
			}
			else
			{
				Debug.LogError($"Command '{entry.Method.Name}' has [TerminalTarget] but no target was parsed. Aborting.");
				return;
			}
		}

		if (entry.IsStatic)
		{
			entry.Method.Invoke(null, args);
			return;
		}

		Component[] instances = _registry.GetInstances(entry.MonoType);

		Component[] filtered = instances;
		if (target.HasValue && target.Value.Value != "@all")
		{
			string targetName = target.Value.Value[1..];
			filtered = instances.Where(c => c.gameObject.name == targetName).ToArray();
		}

		if (filtered.Length == 0)
		{
			string msg = target.HasValue
				? $"No active instance of '{entry.MonoType.Name}' found matching target '{target.Value.Value}'."
				: $"No active instance of '{entry.MonoType.Name}' found in the scene.";
			Debug.LogError(msg);
			return;
		}

		foreach (Component instance in filtered)
		{
			entry.Method.Invoke(instance, args);
		}
	}
}
}