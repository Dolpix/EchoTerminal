using System;

namespace EchoTerminal
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