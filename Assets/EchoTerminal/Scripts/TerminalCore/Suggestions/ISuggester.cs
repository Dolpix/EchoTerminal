using System;
using System.Collections.Generic;

namespace EchoTerminal.TerminalCore
{
public interface ISuggester
{
	IReadOnlyList<string> GetSuggestions(string partial, Type expectedType = null);
}
}