using System;

namespace EchoTerminal
{
public readonly struct CommandParam
{
	public readonly string Name;
	public readonly Type Type;
	public readonly bool IsTarget;

	public CommandParam(string name, Type type, bool isTarget = false)
	{
		Name = name;
		Type = type;
		IsTarget = isTarget;
	}

	public override string ToString()
	{
		return IsTarget ? "<@gameObject>" : $"<{Name}:{Type.Name}>";
	}
}
}