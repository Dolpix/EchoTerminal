using System;
using EchoTerminal.Scripts.TerminalCore.Suggestions;

namespace EchoTerminal.Scripts.TerminalCore.Attributes
{
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class SuggestAttribute : Attribute
{
	public ISuggester Suggester { get; }

	public SuggestAttribute(params string[] values)
	{
		Suggester = new LiteralSuggester(values);
	}

	public SuggestAttribute(Type suggesterType)
	{
		Suggester = (ISuggester)Activator.CreateInstance(suggesterType);
	}
}
}