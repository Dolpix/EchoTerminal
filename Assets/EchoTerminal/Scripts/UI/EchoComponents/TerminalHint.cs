using System.Collections.Generic;
using System.Text;
using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Hints;
using UnityEngine.UIElements;

namespace EchoTerminal.Scripts.UI.EchoComponents
{
public class TerminalHint : IEchoComponent
{
	private const string ColorActive = "#FFFFFF";
	private const string ColorInactive = "#A0A0A0";

	private const string ColorSeparator = "#303030";

	private readonly Label _hintLabel;
	private readonly VisualElement _hintList;

	private readonly TextField _inputField;
	private readonly VisualElement _popup;
	private readonly ScrollView _suggestionList;
	private readonly Terminal _terminal;

	public TerminalHint(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_inputField = root?.Q<TextField>("input-field");
		_popup = root?.Q<VisualElement>("suggestion-popup");
		_hintList = root?.Q<VisualElement>("hint-list");
		_suggestionList = root?.Q<ScrollView>("suggestion-list");

		if (_inputField == null || _hintList == null || _popup == null)
		{
			return;
		}

		_hintLabel = new();
		_hintLabel.AddToClassList("terminal-hint-label");
		_hintList.Add(_hintLabel);

		_inputField.RegisterValueChangedCallback(OnInputChanged);
		_inputField.RegisterCallback<FocusOutEvent>(OnFocusOut);
	}

	~TerminalHint()
	{
		_inputField?.UnregisterValueChangedCallback(OnInputChanged);
		_inputField?.UnregisterCallback<FocusOutEvent>(OnFocusOut);
	}

	private static string BuildRichText(List<List<CommandHintSegment>> rows)
	{
		var sb = new StringBuilder();
		for (var r = 0; r < rows.Count; r++)
		{
			if (r > 0)
			{
				sb.Append($"\n<color={ColorSeparator}>─────────────────</color>\n");
			}

			List<CommandHintSegment> row = rows[r];
			for (var i = 0; i < row.Count; i++)
			{
				if (i > 0)
				{
					sb.Append($"<color={ColorInactive}> </color>");
				}

				CommandHintSegment segment = row[i];
				string color = segment.IsActive ? ColorActive : ColorInactive;
				sb.Append($"<color={color}>{segment.Text}</color>");
			}
		}

		return sb.ToString();
	}

	private void Hide()
	{
		if (_hintList == null)
		{
			return;
		}

		_hintList.style.display = DisplayStyle.None;

		bool suggestionsVisible = _suggestionList is { childCount: > 0 };
		if (suggestionsVisible || _popup == null)
		{
			return;
		}

		_popup.style.display = DisplayStyle.None;
		_popup.RemoveFromClassList("terminal-suggestion-popup--visible");
	}

	private void OnFocusOut(FocusOutEvent evt)
	{
		Hide();
	}

	private void OnInputChanged(ChangeEvent<string> evt)
	{
		Rebuild(evt.newValue);
	}

	private void Rebuild(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			Hide();
			return;
		}

		CommandParseResult result = _terminal.CommandParser.Parse(input);
		List<List<CommandHintSegment>> rows = CommandHintBuilder.Build(input, result, _terminal.Tokenizer);

		if (rows == null || rows.Count == 0)
		{
			Hide();
			return;
		}

		_hintLabel.text = BuildRichText(rows);
		_hintList.style.display = DisplayStyle.Flex;
		_popup.style.display = DisplayStyle.Flex;
		_popup.schedule.Execute(() => _popup.AddToClassList("terminal-suggestion-popup--visible"));
	}
}
}