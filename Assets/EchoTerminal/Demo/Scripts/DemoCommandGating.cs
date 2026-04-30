using System;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using UnityEngine;

namespace EchoTerminal.Demo.Scripts
{
// -- GATED COMMANDS
// These commands exist in the registry from startup but are invisible and
// non-executable until explicitly enabled.
public class DemoCheatCommands : MonoBehaviour
{
	private float _moveSpeed = 5f;
	private bool _noclip;

	// Cheat commands are the main use-case for gating. Tags allow commands to be
	// categorised, and enable/ disable allows us to turn commands off or on depending on
	// if we want people to have access to them. Good way to make dev specific tools.
	//
	// enabled: false  - command starts hidden. The parser won't complete it.
	// [TerminalTag]   - groups it so EnableByTag("cheat") reveals all cheats at once.

	[TerminalCommand(enabled: false)]
	[TerminalTag("cheat")]
	[TerminalDescription("Set movement speed (cheating)")]
	[TerminalTarget]
	private void SetSpeed([Suggest("1", "5", "10", "50")] float speed)
	{
		_moveSpeed = speed;
		Debug.Log($"[{gameObject.name}] Speed → {_moveSpeed}");
	}

	[TerminalCommand(enabled: false)]
	[TerminalTag("cheat")]
	[TerminalDescription("Toggle no-clip movement")]
	[TerminalTarget]
	private void Noclip(bool enabled)
	{
		_noclip = enabled;
		Debug.Log($"[{gameObject.name}] Noclip: {_noclip}");
	}

	[TerminalCommand(enabled: false)]
	[TerminalTag("cheat")]
	[TerminalDescription("Add currency to the player's wallet")]
	[TerminalTarget]
	private void AddGold([Suggest("100", "1000", "9999")] int amount)
	{
		Debug.Log($"[{gameObject.name}] +{amount} gold  (in a real game: wallet += amount)");
	}

	// Debug commands help during development but aren't cheats. You may want to
	// unlock these separately from cheats - e.g., always unlocked in a debug
	// build, locked in a release build.

	[TerminalCommand(enabled: false)]
	[TerminalTag("debug")]
	[TerminalDescription("Log this object's current transform to the console")]
	[TerminalTarget]
	private void PrintTransform()
	{
		Debug.Log($"[{gameObject.name}] pos={transform.position}  " +
				  $"rot={transform.eulerAngles}  scale={transform.localScale}");
	}

	[TerminalCommand(enabled: false)]
	[TerminalTag("debug")]
	[TerminalDescription("Destroy this object (careful!)")]
	[TerminalTarget]
	private void DestroyMe()
	{
		Debug.LogWarning($"[{gameObject.name}] Destroying self via terminal.");
		Destroy(gameObject);
	}

	// A command can belong to multiple tags. It becomes visible when ANY of its
	// tags is enabled, and it is only hidden again if ALL of its tags are disabled.
	//
	//   Unlocked by: "unlock cheats" OR "unlock debug"

	[TerminalCommand(enabled: false)]
	[TerminalTag("cheat", "debug")]
	[TerminalDescription("Teleport to world origin (useful for both cheat and debug)")]
	[TerminalTarget]
	private void ResetPosition()
	{
		transform.position = Vector3.zero;
		Debug.Log($"[{gameObject.name}] Reset to origin.");
	}

	// Not everything needs to be gated. Commands without enabled: false are
	// visible from the start. You can freely mix gated and always-on commands
	// on the same MonoBehaviour.

	[TerminalCommand]
	[TerminalDescription("Print this object's name and component list")]
	[TerminalTarget]
	private void Inspect()
	{
		Component[] components = GetComponents<Component>();
		Debug.Log(
			$"[{gameObject.name}] has {components.Length} component(s): " +
			string.Join(", ", Array.ConvertAll(components, c => c.GetType().Name))
		);
	}
}
}