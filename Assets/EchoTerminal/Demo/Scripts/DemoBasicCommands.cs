using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using UnityEngine;

namespace EchoTerminal.Demo.Scripts
{
// -- STATIC COMMANDS
// A plain static class is all you need. No MonoBehaviour, no scene setup.
// Static commands are perfect for global utilities that don't belong to any
// specific game object: math helpers, save/load triggers, config toggles, etc.
public static class DemoStaticCommands
{
	// The simplest possible command: no parameters at all.
	// Type "Ping" in the terminal and press Enter.
	//
	// Without [TerminalDescription] the command still works, it just won't
	// show a description in the `help` output.
	[TerminalCommand]
	[TerminalDescription("Reply with Pong")]
	private static void Ping()
	{
		Debug.Log("Pong!");
	}

	// A single string parameter. The terminal collects everything after the
	// command name as the string value, including spaces if quoted.
	//   Type:  Echo Hello            → logs  Hello
	//   Type:  Echo "Hello World"    → logs  Hello World
	[TerminalCommand]
	[TerminalDescription("Echo a message back to the log")]
	private static void Echo(string message)
	{
		Debug.Log(message);
	}

	// Multiple parameters of the same type are separated by spaces.
	//   Type:  Add 3 7    → logs  3 + 7 = 10
	[TerminalCommand]
	[TerminalDescription("Log the sum of two integers")]
	private static void Add(int a, int b)
	{
		Debug.Log($"{a} + {b} = {a + b}");
	}

	// The name passed to [TerminalCommand] overrides the C# method name.
	// Use this when the ideal terminal name and the ideal code name differ.
	//   Type:  Clamp 2.5 0 1    → logs  Clamped: 1
	[TerminalCommand("Clamp")]
	[TerminalDescription("Clamp a float between min and max")]
	private static void ClampFloat(float value, float min, float max)
	{
		Debug.Log($"Clamped: {Mathf.Clamp(value, min, max)}");
	}

	// [Inject] marks a parameter that the terminal fills in automatically
	// the user never types it. The terminal's injection container provides a
	// Terminal reference, which lets commands inspect the registry, log
	// formatted output, or read other terminal state.
	//
	//   Type:  CommandCount    → logs  "12 command(s) currently enabled."
	[TerminalCommand]
	[TerminalDescription("Log the number of currently enabled commands")]
	private static void CommandCount([Inject] Terminal terminal)
	{
		int count = terminal.Registry.GetCommandNames().Count;
		Debug.Log($"{count} command(s) currently enabled.");
	}
}


// -- INSTANCE COMMANDS
// Attach this MonoBehaviour to one or more GameObjects.
// When an instance command is submitted, the terminal finds every active
// instance of this class in the scene and calls the method on each one.
//
// This is useful for broadcast operations ("mute all audio sources") but can
// be surprising if you expect only one object to respond.
// Use [TerminalTarget] when you want per-object control — see DemoTargeting.cs.
public class DemoInstanceCommands : MonoBehaviour
{
	private bool _invincible;

	// Attach this script to a GameObject called "Player".
	// Type: SetInvincible true
	// Every DemoInstanceCommands in the scene will receive this call.
	[TerminalCommand]
	[TerminalDescription("Toggle invincibility on this object")]
	private void SetInvincible(bool invincibleEnabled)
	{
		_invincible = invincibleEnabled;
		Debug.Log($"[{gameObject.name}] Invincible: {_invincible}");
	}

	// Struct types like Vector3 are parsed from space-separated components.
	//   Type:  Warp (10, 0, 5)    → teleports this object's position
	//   Type:  Warp (0, 0, 0)     → resets to world origin
	[TerminalCommand]
	[TerminalDescription("Warp this object to a world position")]
	private void Warp(Vector3 position)
	{
		transform.position = position;
		Debug.Log($"[{gameObject.name}] Warped to {transform.position}");
	}

	// Commands on instances can still use [Inject] exactly like static ones.
	// Here we read the terminal registry to confirm the command exists.
	[TerminalCommand]
	[TerminalDescription("Print this object's name to the log")]
	private void WhoAmI([Inject] Terminal terminal)
	{
		Debug.Log(
			$"I am: {gameObject.name} ({GetType().Name}) — terminal has " +
			$"{terminal.Registry.GetCommandNames().Count} enabled commands."
		);
	}
}
}