using System;
using System.Collections.Generic;
using EchoTerminal.TerminalCore;
using UnityEngine;

namespace EchoTerminal
{
public class Terminal
{
	public CommandParser CommandParser { get; }

	public IReadOnlyList<TerminalEntry> Entries => _entries;
	public CommandRegistry Registry { get; }
	public SuggestorRegistry Suggestors { get; }
	public Tokenizer Tokenizer { get; }
	public event Action OnCleared;
	public event Action<TerminalEntry> OnEntryAdded;
	public event Action OnSubmitted;

	private readonly List<TerminalEntry> _entries = new();
	private readonly CommandExecutor _executor;
	private readonly int _maxEntries;

	public Terminal(int maxEntries = 1000)
	{
		_maxEntries = maxEntries;
		Registry = new CommandRegistry();
		Registry.Scan();
		Suggestors = new SuggestorRegistry();
		Suggestors.Scan(Registry);
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(Registry.GetCommandNames()));
		Tokenizer = new Tokenizer(ParserRegistry.CreateAllParsers());
		CommandParser = new CommandParser(Registry, Tokenizer);
		_executor = new CommandExecutor(CommandParser, Registry, Tokenizer);
		BindCommand.Terminal = this;
	}

	public void Clear()
	{
		_entries.Clear();
		OnCleared?.Invoke();
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

	public void Submit(string input)
	{
		OnSubmitted?.Invoke();
		Log(input, kind: LogKind.Command);
		_executor.Execute(input);
	}
}
}