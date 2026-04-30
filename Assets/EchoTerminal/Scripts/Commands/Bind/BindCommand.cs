using System.Collections.Generic;
using System.Text;
using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoTerminal.Scripts.Commands.Bind
{
public static class BindCommand
{
	public static Terminal Terminal { get; set; }

	[TerminalCommand("BindsAdd")]
	[TerminalDescription("Add a command to be activated on key press")]
	private static void BindsAdd(Key key, BoundCommand command)
	{
		if (command.Tokens.Count == 0 ||
			command.Tokens[0].ExpectedType != typeof(CommandName) ||
			command.Tokens[0].State == TokenState.Failed)
		{
			Debug.LogError($"'{command.Raw}' is not a recognised command.");
			return;
		}

		if (Terminal != null)
		{
			CommandParseResult parseResult =
				new CommandParser(Terminal.Registry, Terminal.Tokenizer).Parse(command.Raw);
			if (!parseResult.IsMatch)
			{
				Debug.LogError($"Cannot bind — {parseResult.GetError()}");
				return;
			}
		}

		Dictionary<Key, string> existing = BindStore.GetAll();
		bool wasRebound = existing.TryGetValue(key, out string previous);

		BindStore.Set(key, command.Raw);

		Debug.Log(wasRebound
			? $"Rebound '{key}': '{previous}' → '{command.Raw}'"
			: $"Bound '{key}' to '{command.Raw}'.");
	}

	[TerminalCommand("BindsRemove")]
	[TerminalDescription("Remove a command to be activated on key press")]
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

	[TerminalCommand("BindsClear")]
	[TerminalDescription("Clear all bindings")]
	private static void BindsClear()
	{
		int count = BindStore.GetAll().Count;

		if (count == 0)
		{
			Debug.Log("No bindings to clear.");
			return;
		}

		BindStore.Clear();
		Debug.Log($"Cleared {count} binding{(count == 1 ? "" : "s")}.");
	}

	[TerminalCommand("BindsLog")]
	[TerminalDescription("Log all bindings")]
	private static void BindsLog()
	{
		Dictionary<Key, string> all = BindStore.GetAll();

		if (all.Count == 0)
		{
			Debug.Log("No bindings set.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Bindings ({all.Count}):");

		foreach ((Key key, string command) in all)
		{
			sb.AppendLine($"  {key,-12} → {command}");
		}

		Debug.Log(sb.ToString().TrimEnd());
	}
}
}