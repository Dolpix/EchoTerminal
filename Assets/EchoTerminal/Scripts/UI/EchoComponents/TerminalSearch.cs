using System;
using EchoTerminal.Scripts.TerminalCore;
using EchoTerminal.Scripts.TerminalCore.Structs;
using UnityEngine.UIElements;

namespace EchoTerminal.Scripts.UI.EchoComponents
{
public class TerminalSearch : IEchoComponent
{
	private const string _hiddenClass = "terminal-entry--search-hidden";

	private readonly VisualElement _logContainer;
	private readonly TextField _searchField;
	private readonly Terminal _terminal;

	private string _query = "";

	public TerminalSearch(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_logContainer = root?.Q<VisualElement>("log-container");
		_searchField = root?.Q<TextField>("search-field");

		_searchField?.RegisterValueChangedCallback(OnSearchChanged);

		_terminal.OnEntryAdded += OnEntryAdded;
		_terminal.OnCleared += OnCleared;
	}

	~TerminalSearch()
	{
		_terminal.OnEntryAdded -= OnEntryAdded;
		_terminal.OnCleared -= OnCleared;
	}

	private void OnSearchChanged(ChangeEvent<string> evt)
	{
		_query = evt.newValue ?? "";
		ApplyFilter();
	}

	private void OnEntryAdded(TerminalEntry entry)
	{
		if (string.IsNullOrEmpty(_query))
		{
			return;
		}

		VisualElement last = _logContainer?.childCount > 0
			? _logContainer[_logContainer.childCount - 1]
			: null;

		if (last != null)
		{
			ApplyToElement(last);
		}
	}

	private void OnCleared()
	{
		_query = "";
		if (_searchField != null)
		{
			_searchField.SetValueWithoutNotify("");
		}
	}

	private void ApplyFilter()
	{
		if (_logContainer == null)
		{
			return;
		}

		foreach (VisualElement child in _logContainer.Children())
		{
			ApplyToElement(child);
		}
	}

	private void ApplyToElement(VisualElement element)
	{
		if (string.IsNullOrEmpty(_query))
		{
			element.RemoveFromClassList(_hiddenClass);
			return;
		}

		var message = element.Q<Label>("message");
		string text = message?.text ?? "";
		bool matches = text.IndexOf(_query, StringComparison.OrdinalIgnoreCase) >= 0;
		element.EnableInClassList(_hiddenClass, !matches);
	}
}
}