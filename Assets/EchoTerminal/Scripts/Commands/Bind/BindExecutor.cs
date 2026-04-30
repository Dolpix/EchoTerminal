using System.Collections.Generic;
using EchoTerminal.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EchoTerminal.Scripts.Commands.Bind
{
public class BindExecutor : MonoBehaviour
{
	private Dictionary<Key, string> _binds;
	private GameTerminalUI _terminalUI;

	private void Update()
	{
		if (_terminalUI == null || _terminalUI.Terminal == null)
		{
			return;
		}

		if (GameTerminalUI.IsFocused)
		{
			return;
		}

		Keyboard keyboard = Keyboard.current;

		if (keyboard == null)
		{
			return;
		}

		foreach ((Key key, string command) in _binds)
		{
			if (keyboard[key].wasPressedThisFrame)
			{
				_terminalUI.Terminal.Submit(command);
			}
		}
	}

	private void OnDestroy()
	{
		BindStore.Changed -= RebuildCache;
	}

	public void Init(GameTerminalUI terminalUI)
	{
		_terminalUI = terminalUI;
		RebuildCache();
		BindStore.Changed += RebuildCache;
	}

	private void RebuildCache()
	{
		_binds = BindStore.GetAll();
	}
}
}