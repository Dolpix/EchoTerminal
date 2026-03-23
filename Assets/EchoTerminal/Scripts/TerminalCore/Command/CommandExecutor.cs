using UnityEngine;

namespace EchoTerminal
{
public class CommandExecutor
{
	private readonly CommandParser _parser;
	private readonly Terminal _terminal;

	public CommandExecutor(Terminal terminal, CommandParser parser)
	{
		_terminal = terminal;
		_parser = parser;
	}

	public void Execute(string input)
	{
		var result = _parser.Parse(input);

		if (string.IsNullOrEmpty(result.CommandName)) return;

		if (!result.IsKnownCommand)
		{
			_terminal.Log($"Unknown command: '{result.CommandName}'");
			return;
		}

		// Complete overload — invoke immediately.
		foreach (var overload in result.Overloads)
		{
			if (overload.IsComplete) { Invoke(overload); return; }
		}

		// Bad @target name typed — report it before a generic error.
		foreach (var overload in result.Overloads)
		{
			var p = overload.Params;
			if (p.Count > 0 && p[0].Expected.IsTarget && p[0].Token != null && !p[0].IsValid)
			{
				_terminal.Log($"No GameObject named '{p[0].Token[1..]}' found in scene.");
				return;
			}
		}

		// No @target given — broadcast to all matching instances if remaining args are valid.
		foreach (var overload in result.Overloads)
		{
			var p = overload.Params;
			if (p.Count == 0 || !p[0].Expected.IsTarget || p[0].Token != null) continue;

			var restValid = true;
			for (var i = 1; i < p.Count; i++) if (!p[i].IsValid) { restValid = false; break; }
			if (restValid) { Invoke(overload); return; }
		}

		_terminal.Log($"Invalid arguments for '{result.CommandName}'");
	}

	private void Invoke(OverloadResult overload)
	{
		var entry = overload.Entry;
		var methodParams = entry.Method.GetParameters();
		var args = new object[methodParams.Length];
		var hasTarget = overload.Params.Count > 0 && overload.Params[0].Expected.IsTarget;
		var offset = hasTarget ? 1 : 0;
		var userIdx = 0;

		for (var i = 0; i < methodParams.Length; i++)
		{
			args[i] = methodParams[i].ParameterType == typeof(Terminal)
				? _terminal
				: overload.Params[offset + userIdx++].Value;
		}

		if (entry.IsStatic)
		{
			if (entry.Method.Invoke(null, args) is string msg) _terminal.Log(msg);
			return;
		}

		var singleTarget = hasTarget ? overload.Params[0].Value as GameObject : null;
		var invoked = false;

		foreach (var target in _parser.Registry.GetInstances(entry.MonoType))
		{
			if (singleTarget != null && target.gameObject != singleTarget) continue;
			invoked = true;
			if (entry.Method.Invoke(target, args) is string msg) _terminal.Log(msg);
		}

		if (!invoked)
		{
			_terminal.Log(singleTarget != null
				? $"No '{entry.MonoType.Name}' on '{singleTarget.name}' found in scene."
				: $"No active '{entry.MonoType.Name}' found in scene.");
		}
	}
}
}
