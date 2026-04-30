using EchoTerminal.Scripts.ScriptableObjects;
using EchoTerminal.Scripts.TerminalCore;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Scripts.UI.EchoComponents
{
public class TerminalInput : IEchoComponent
{
	private readonly TextField _inputField;
	private readonly Terminal _terminal;

	public TerminalInput(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_inputField = root?.Q<TextField>("input-field");
		_inputField?.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
	}

	~TerminalInput()
	{
		_inputField?.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
		{
			return;
		}

		string text = _inputField.value;
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		_terminal.Submit(text);
		_inputField.value = "";

		evt.StopImmediatePropagation();
		evt.StopPropagation();

		_inputField.schedule.Execute(() => _inputField.Focus());
	}
}
}