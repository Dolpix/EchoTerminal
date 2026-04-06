using System.Text;
using EchoTerminal.TerminalCore;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoTerminal
{
public static class BindCommand
{
	public static Terminal Terminal { get; set; }

	[TerminalCommand("BindsAdd", "Bind a key to a command. Usage: BindsAdd <key> ><command args><")]
	private static void BindsAdd(Key key, BoundCommand command)
	{
		if (command.Tokens.Count == 0 || command.Tokens[0].Type != typeof(CommandName))
		{
			Debug.LogError($"'{command.Raw}' is not a recognised command.");
			return;
		}

		if (Terminal != null && !Terminal.TryValidateCommand(command.Raw, out var error))
		{
			Debug.LogError($"Cannot bind — {error}");
			return;
		}

		var existing = BindStore.GetAll();
		var wasRebound = existing.TryGetValue(key, out var previous);

		BindStore.Set(key, command.Raw);

		Debug.Log(wasRebound
			? $"Rebound '{key}': '{previous}' → '{command.Raw}'"
			: $"Bound '{key}' to '{command.Raw}'.");
	}

	[TerminalCommand("BindsRemove", "Remove the binding for a key")]
	private static void BindsRemove(Key key)
	{
		if (!BindStore.GetAll().ContainsKey(key))
		{
			Debug.LogWarning($"'{key}' has no binding to remove.");
			return;
		}

		BindStore.Remove(key);
		Debug.Log($"Removed binding for '{key}'.");
	}

	[TerminalCommand("BindsClear", "Remove all key bindings")]
	private static void BindsClear()
	{
		var count = BindStore.GetAll().Count;

		if (count == 0)
		{
			Debug.Log("No bindings to clear.");
			return;
		}

		BindStore.Clear();
		Debug.Log($"Cleared {count} binding{(count == 1 ? "" : "s")}.");
	}

	[TerminalCommand("BindsLog", "List all current key bindings")]
	private static void BindsLog()
	{
		var all = BindStore.GetAll();

		if (all.Count == 0)
		{
			Debug.Log("No bindings set.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Bindings ({all.Count}):");

		foreach (var (key, command) in all)
		{
			sb.AppendLine($"  {key,-12} → {command}");
		}

		Debug.Log(sb.ToString().TrimEnd());
	}
}
}