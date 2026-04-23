using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
public class TerminalHighlighter : IEchoComponent
{
	private readonly TerminalHighlighterCore _core;
	private readonly Label _highlightLabel;
	private readonly TextField _inputField;

	public TerminalHighlighter(Terminal terminal, VisualElement root)
	{
		_core = terminal.HighlighterCore;
		_inputField = root?.Q<TextField>("input-field");
		_highlightLabel = root?.Q<Label>("highlight-label");

		if (_inputField == null || _highlightLabel == null)
		{
			return;
		}

		_inputField.RegisterValueChangedCallback(OnInputChanged);
	}

	private void OnInputChanged(ChangeEvent<string> evt)
	{
		_highlightLabel.text = string.IsNullOrEmpty(evt.newValue)
			? string.Empty
			: _core.BuildHighlightedText(evt.newValue);
	}

	~TerminalHighlighter()
	{
		_inputField?.UnregisterValueChangedCallback(OnInputChanged);
	}
}
}