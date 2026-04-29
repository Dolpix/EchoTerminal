using System;
using UnityEngine;

namespace EchoTerminal.Demos
{
// -- PASSPHRASE UNLOCKER
// Attach this to any GameObject and wire up the terminal reference.
// When the user submits a matching passphrase, the corresponding tag group
// is revealed in the terminal.
//
// This is one unlock strategy. The sky is the limit, here's some examples:
//   • CommandUnlocker.cs  — text passphrase (submitted as a command)
//   • KonamiCommandUnlocker.cs — key sequence (↑ ↑ ↓ ↓ ← → ← → B A)
//
// You can invent your own: achievement callback, network flag, editor pref, etc.
public class DemoPassphraseUnlocker : MonoBehaviour
{
	// Separate passphrases unlock separate tag groups. Change these to anything
	// you like - they won't appear in help or autocomplete while disabled.
	private const string _cheatPhrase = "unlock cheats";
	private const string _debugPhrase = "unlock debug";
	private const string _allPhrase = "unlock all";
	[SerializeField] private GameTerminalUI _terminal;

	private void Start()
	{
		if (_terminal == null)
		{
			Debug.LogWarning("[DemoPassphraseUnlocker] No terminal assigned.");
			return;
		}

		// OnCommandSubmitted fires after every submitted line, whether
		// it matched a registered command.
		_terminal.Terminal.OnCommandSubmitted += OnSubmitted;
	}

	private void OnDestroy()
	{
		if (_terminal != null && _terminal.Terminal != null)
		{
			_terminal.Terminal.OnCommandSubmitted -= OnSubmitted;
		}
	}

	private void OnSubmitted(string input)
	{
		string trimmed = input.Trim();
		CommandRegistry registry = _terminal.Terminal.Registry;

		if (string.Equals(trimmed, _cheatPhrase, StringComparison.OrdinalIgnoreCase))
		{
			registry.EnableByTag("cheat");
			Debug.Log("Cheat commands unlocked. Type 'help' to see them.");
			return;
		}

		if (string.Equals(trimmed, _debugPhrase, StringComparison.OrdinalIgnoreCase))
		{
			registry.EnableByTag("debug");
			Debug.Log("Debug commands unlocked. Type 'help' to see them.");
			return;
		}

		if (string.Equals(trimmed, _allPhrase, StringComparison.OrdinalIgnoreCase))
		{
			foreach (string commandName in registry.GetAllCommandNames())
			{
				registry.Enable(commandName);
			}

			Debug.Log("All commands unlocked.");
		}
	}


	// -- RE-LOCKING AT RUNTIME
	// You can also disable commands after enabling them, e.g. when the player
	// enters a cutscene, load screen, or restricted area. Expose these via your
	// own game events rather than terminal commands if players shouldn't see them.

	// Re-hides all cheat commands. Call this from your own game logic as needed.
	public void LockCheats()
	{
		if (_terminal == null)
		{
			return;
		}

		_terminal.Terminal.Registry.DisableByTag("cheat");
		Debug.Log("[DemoPassphraseUnlocker] Cheat commands locked.");
	}

	// Checks whether a specific command is currently reachable by the user.
	// Useful for conditional UI (e.g., showing a "cheats active" watermark).
	public bool AreCheatsUnlocked()
	{
		if (_terminal == null)
		{
			return false;
		}

		// Check any representative cheat command - if one is enabled, the group is.
		return _terminal.Terminal.Registry.IsEnabled("SetSpeed");
	}
}
}