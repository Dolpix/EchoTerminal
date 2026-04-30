using System.Text;
using System.Text.RegularExpressions;
using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Enum;
using EchoTerminal.Scripts.TerminalCore.Structs;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Scripts.UI.EchoComponents
{
public class TerminalCopyButton : IEchoComponent
{
	private static readonly Regex RichTextTags = new("<[^>]+>", RegexOptions.Compiled);

	private readonly Button _copyButton;
	private readonly Label _copyToast;
	private readonly VisualElement _logContainer;
	private readonly Terminal _terminal;

	public TerminalCopyButton(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_logContainer = root?.Q<VisualElement>("log-container");
		_copyButton = root?.Q<Button>("copy-button");

		_copyToast = new("Copied to clipboard");
		_copyToast.AddToClassList("terminal-copy-toast");
		_copyToast.pickingMode = PickingMode.Ignore;
		VisualElement toastParent = root?.Q<VisualElement>("terminal-content") ?? root;
		toastParent?.Add(_copyToast);

		_copyButton?.RegisterCallback<ClickEvent>(OnCopyClicked);
	}

	~TerminalCopyButton()
	{
		_copyButton?.UnregisterCallback<ClickEvent>(OnCopyClicked);
	}

	private void OnCopyClicked(ClickEvent evt)
	{
		bool hideLog = _logContainer?.ClassListContains("filter-hide-log") ?? false;
		bool hideWarning = _logContainer?.ClassListContains("filter-hide-warning") ?? false;
		bool hideError = _logContainer?.ClassListContains("filter-hide-error") ?? false;
		bool hideCommand = _logContainer?.ClassListContains("filter-hide-command") ?? false;
		bool showTimestamp = _logContainer?.ClassListContains("timestamps-visible") ?? false;

		var sb = new StringBuilder();

		foreach (TerminalEntry entry in _terminal.Entries)
		{
			if (hideLog && entry.Kind == LogKind.Log)
			{
				continue;
			}

			if (hideWarning && entry.Kind == LogKind.Warning)
			{
				continue;
			}

			if (hideError && entry.Kind == LogKind.Error)
			{
				continue;
			}

			if (hideCommand && entry.Kind == LogKind.Command)
			{
				continue;
			}

			if (showTimestamp)
			{
				sb.Append($"[{entry.Timestamp:HH:mm:ss}] ");
			}

			sb.AppendLine(RichTextTags.Replace(entry.Text, ""));
		}

		GUIUtility.systemCopyBuffer = sb.ToString().TrimEnd();

		_copyToast.AddToClassList("terminal-copy-toast--visible");
		_copyToast.schedule.Execute(() => _copyToast.RemoveFromClassList("terminal-copy-toast--visible")) .StartingIn(1500);
	}
}
}