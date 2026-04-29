using System;

namespace EchoTerminal
{
[AttributeUsage(AttributeTargets.Method)]
public sealed class TerminalDescriptionAttribute : Attribute
{
	public string Description { get; }

	public TerminalDescriptionAttribute(string description)
	{
		Description = description;
	}
}
}