using System;

namespace EchoTerminal
{
[AttributeUsage(AttributeTargets.Method)]
public sealed class TerminalCommandAttribute : Attribute
{
	public string Name { get; }
	public bool Enabled { get; }

	public TerminalCommandAttribute(string name = null, bool enabled = true)
	{
		Name = name;
		Enabled = enabled;
	}
}
}