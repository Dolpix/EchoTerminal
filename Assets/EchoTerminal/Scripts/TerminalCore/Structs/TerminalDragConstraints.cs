using System;

namespace EchoTerminal
{
[Serializable]
public struct TerminalDragConstraints
{
	public TerminalEdgeConstraints Top;
	public TerminalEdgeConstraints Left;
	public TerminalEdgeConstraints Right;
	public TerminalEdgeConstraints Bottom;
}
}
