using System;

namespace EchoTerminal
{
[AttributeUsage(AttributeTargets.Method)]
public sealed class TerminalTargetAttribute : Attribute
{
}
}