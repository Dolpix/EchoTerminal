using System;
using System.Collections.Generic;
using EchoTerminal.TerminalCore;
using UnityEngine;

namespace EchoTerminal
{
public class Terminal
{
	public CommandRegistry Registry { get; }
	public IReadOnlyList<TerminalEntry> Entries => _entries;

	public event Action OnCleared;
	public event Action<TerminalEntry> OnEntryAdded;
	public event Action OnSubmitted;

	private readonly List<TerminalEntry> _entries = new();
	private readonly List<ITokenParser> _parsers;
	private readonly int _maxEntries;

	public Terminal(int maxEntries = 1000)
	{
		_maxEntries = maxEntries;
		Registry = new();
		Registry.Scan();
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(Registry.GetCommandNames()));
		_parsers = ParserRegistry.CreateAll();
	}

	public void Submit(string input)
	{
		OnSubmitted?.Invoke();
		Log(input, kind: LogKind.Command);
	}

	public void Log(string text, Color? color = null, LogKind kind = LogKind.Log)
	{
		var entry = new TerminalEntry(text, color ?? Color.white, kind);
		if (_entries.Count >= _maxEntries)
		{
			_entries.RemoveAt(0);
		}

		_entries.Add(entry);
		OnEntryAdded?.Invoke(entry);
	}

	public void Clear()
	{
		_entries.Clear();
		OnCleared?.Invoke();
	}
}
}