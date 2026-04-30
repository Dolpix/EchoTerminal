using System;

namespace EchoTerminal.Scripts.TerminalCore.Attributes
{
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class SuggestorForAttribute : Attribute
{
	public Type TokenType { get; }

	public SuggestorForAttribute(Type tokenType)
	{
		TokenType = tokenType;
	}
}
}