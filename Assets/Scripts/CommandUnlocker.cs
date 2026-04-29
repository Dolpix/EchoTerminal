using EchoTerminal;
using UnityEngine;

namespace DefaultNamespace
{
public class CommandUnlocker : MonoBehaviour
{
    [SerializeField] private GameTerminalUI _terminal;
    [SerializeField] private string _passphrase = "unlock";

    [Tooltip("Tags to unlock. Leave empty to unlock all commands.")]
    [SerializeField] private string[] _tags = System.Array.Empty<string>();

    private void Start()
    {
        if (_terminal == null)
        {
            Debug.LogWarning("[CommandUnlocker] No GameTerminalUI assigned.");
            return;
        }

        _terminal.Terminal.OnCommandSubmitted += OnCommandSubmitted;
    }

    private void OnDestroy()
    {
        if (_terminal != null && _terminal.Terminal != null)
        {
            _terminal.Terminal.OnCommandSubmitted -= OnCommandSubmitted;
        }
    }

    private void OnCommandSubmitted(string input)
    {
        if (!string.Equals(input.Trim(), _passphrase, System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var registry = _terminal.Terminal.Registry;

        if (_tags.Length == 0)
        {
            foreach (var name in registry.GetAllCommandNames())
            {
                registry.Enable(name);
            }
            Debug.Log("[CommandUnlocker] All commands unlocked.");
        }
        else
        {
            foreach (var tag in _tags)
            {
                registry.EnableByTag(tag);
            }
            Debug.Log($"[CommandUnlocker] Unlocked commands with tags: {string.Join(", ", _tags)}");
        }
    }
}
}
