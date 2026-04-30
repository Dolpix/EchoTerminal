using System;
using System.Reflection;

namespace EchoTerminal.Scripts.TerminalCore.Structs
{
public readonly struct CommandEntry
{
	public readonly MethodInfo Method;
	public readonly Type MonoType;
	public readonly bool HasTarget;

	public bool IsStatic => MonoType == null;

	public CommandEntry(MethodInfo method, Type monoType, bool hasTarget = false)
	{
		Method = method;
		MonoType = monoType;
		HasTarget = hasTarget;
	}
}
}