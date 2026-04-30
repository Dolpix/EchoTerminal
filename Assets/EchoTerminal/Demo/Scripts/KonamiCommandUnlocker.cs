using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace EchoTerminal.Demo.Scripts
{
    public class KonamiCommandUnlocker : MonoBehaviour
    {
        private static readonly Key[] KonamiSequence =
        {
            Key.UpArrow, Key.UpArrow,
            Key.DownArrow, Key.DownArrow,
            Key.LeftArrow, Key.RightArrow,
            Key.LeftArrow, Key.RightArrow,
            Key.B, Key.A
        };

        [SerializeField] private GameTerminalUI _terminal;

        [Tooltip("Tags to unlock. Leave empty to unlock all commands.")]
        [SerializeField] private string[] _tags = System.Array.Empty<string>();

        private int _progress;

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            Key expected = KonamiSequence[_progress];

            if (keyboard[expected].wasPressedThisFrame)
            {
                _progress++;
                if (_progress != KonamiSequence.Length)
                {
                    return;
                }

                _progress = 0;
                UnlockAll();
            }
            else
            {
                foreach (KeyControl unused in keyboard.allKeys.Where(key => key.wasPressedThisFrame))
                {
                    _progress = 0;
                    break;
                }
            }
        }

        private void UnlockAll()
        {
            if (_terminal == null || _terminal.Terminal == null)
            {
                Debug.LogWarning("[KonamiCommandUnlocker] No terminal assigned.");
                return;
            }

            CommandRegistry registry = _terminal.Terminal.Registry;

            if (_tags.Length == 0)
            {
                foreach (string commandName in registry.GetAllCommandNames())
                {
                    registry.Enable(commandName);
                }
                Debug.Log("[KonamiCommandUnlocker] Konami code entered — all commands unlocked.");
            }
            else
            {
                foreach (string t in _tags)
                {
                    registry.EnableByTag(t);
                }
                Debug.Log($"[KonamiCommandUnlocker] Konami code entered — unlocked commands with tags: {string.Join(", ", _tags)}");
            }
        }
    }
}