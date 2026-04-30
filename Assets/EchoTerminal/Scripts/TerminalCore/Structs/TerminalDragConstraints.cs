using System;

namespace EchoTerminal.Scripts.TerminalCore.Structs
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