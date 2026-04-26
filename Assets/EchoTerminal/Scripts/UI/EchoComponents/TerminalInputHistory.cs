using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
public class TerminalInputHistory : IEchoComponent
{
	private readonly TextField _inputField;
	private readonly Terminal _terminal;
	private readonly List<string> _history = new();
	private int _historyIndex = -1;
	private string _draft = string.Empty;

	public TerminalInputHistory(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_inputField = root?.Q<TextField>("input-field");

		if (_inputField == null)
		{
			return;
		}

		_terminal.OnCommandSubmitted += OnCommandSubmitted;
		_inputField.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
	}

	private void OnCommandSubmitted(string command)
	{
		if (_history.Count == 0 || _history[^1] != command)
		{
			_history.Add(command);
		}

		_historyIndex = -1;
		_draft = string.Empty;
	}

	private void OnKeyDown(KeyDownEvent evt)
	{
		if (!evt.ctrlKey)
		{
			return;
		}

		switch (evt.keyCode)
		{
			case KeyCode.UpArrow:
				Navigate(-1);
				evt.StopImmediatePropagation();
				evt.StopPropagation();
				break;
			case KeyCode.DownArrow:
				Navigate(1);
				evt.StopImmediatePropagation();
				evt.StopPropagation();
				break;
		}
	}

	private void Navigate(int direction)
	{
		if (_history.Count == 0)
		{
			return;
		}

		if (_historyIndex == -1)
		{
			if (direction > 0)
			{
				return;
			}

			_draft = _inputField.value;
			_historyIndex = _history.Count - 1;
		}
		else
		{
			_historyIndex += direction;
		}

		if (_historyIndex < 0)
		{
			_historyIndex = 0;
		}
		else if (_historyIndex >= _history.Count)
		{
			_historyIndex = -1;
			SetInput(_draft);
			return;
		}

		SetInput(_history[_historyIndex]);
	}

	private void SetInput(string value)
	{
		_inputField.value = value;
		_inputField.schedule.Execute(() =>
		{
			int len = _inputField.value.Length;
			_inputField.SelectRange(len, len);
		});
	}

	~TerminalInputHistory()
	{
		_terminal.OnCommandSubmitted -= OnCommandSubmitted;
		_inputField?.UnregisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
	}
}
}