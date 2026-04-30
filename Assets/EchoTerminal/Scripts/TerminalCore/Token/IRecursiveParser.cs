using System;
using System.Collections.Generic;

namespace EchoTerminal.Scripts.TerminalCore.Token
{
public interface IRecursiveParser
{
	List<Token> GetSubTokens(string raw, Type expectedType);
}
}