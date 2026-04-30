using System;

namespace EchoTerminal.Scripts.TerminalCore.Attributes
{
[AttributeUsage(AttributeTargets.Method)]
public sealed class TerminalTagAttribute : Attribute
{
	public string[] Tags { get; }

	public TerminalTagAttribute(params string[] tags)
	{
		Tags = tags;
	}
}
}