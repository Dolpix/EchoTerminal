using System;
using System.Collections.Generic;

namespace EchoTerminal
{
public struct CommandParseResult
{
	public static readonly CommandParseResult Empty = new(string.Empty, false, Array.Empty<OverloadResult>());

	public string CommandName { get; }
	public bool IsKnownCommand { get; }
	public IReadOnlyList<OverloadResult> Overloads { get; }

	public CommandParseResult(string commandName, bool isKnownCommand, IReadOnlyList<OverloadResult> overloads)
	{
		CommandName = commandName;
		IsKnownCommand = isKnownCommand;
		Overloads = overloads;
	}
}
}