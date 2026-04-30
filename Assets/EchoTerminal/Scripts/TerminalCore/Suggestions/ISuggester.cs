using System;
using System.Collections.Generic;

namespace EchoTerminal.Scripts.TerminalCore.Suggestions
{
public interface ISuggester
{
	IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null);
}
}