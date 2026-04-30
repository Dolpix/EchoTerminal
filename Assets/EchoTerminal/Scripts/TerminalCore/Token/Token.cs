using System;

namespace EchoTerminal.Scripts.TerminalCore.Token
{
public struct Token
{
	public string Raw;
	public TokenState State;
	public Type ExpectedType;
}
}