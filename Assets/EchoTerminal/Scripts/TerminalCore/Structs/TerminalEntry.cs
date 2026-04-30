using System;
using EchoTerminal.Scripts.TerminalCore.Enum;
using UnityEngine;

namespace EchoTerminal.Scripts.TerminalCore.Structs
{
public readonly struct TerminalEntry
{
	public readonly string Text;
	public readonly Color Color;
	public readonly DateTime Timestamp;
	public readonly LogKind Kind;

	public TerminalEntry(string text, Color color, LogKind kind = LogKind.Log)
	{
		Text = text;
		Color = color;
		Timestamp = DateTime.Now;
		Kind = kind;
	}
}
}