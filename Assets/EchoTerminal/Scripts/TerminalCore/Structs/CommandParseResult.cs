using System;
using System.Collections.Generic;

namespace EchoTerminal
{
public struct CommandParseResult
{
	public static readonly CommandParseResult Empty =
		new(
			string.Empty,
			false,
			null,
			0,
			Array.Empty<OverloadResult>()
		);

	public string CommandName { get; }
	public bool IsKnownCommand { get; }
	public string Args { get; }
	public int LeadingSpaces { get; }
	public IReadOnlyList<OverloadResult> Overloads { get; }

	public CommandParseResult(
		string commandName,
		bool isKnownCommand,
		string args,
		int leadingSpaces,
		IReadOnlyList<OverloadResult> overloads)
	{
		CommandName = commandName;
		IsKnownCommand = isKnownCommand;
		Args = args;
		LeadingSpaces = leadingSpaces;
		Overloads = overloads;
	}
}
}