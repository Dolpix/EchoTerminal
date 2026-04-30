using EchoTerminal.Scripts.TerminalCore.Token;
using UnityEngine;

namespace EchoTerminal.Scripts.ScriptableObjects.Highlighting
{
public abstract class TokenHighlighter : ScriptableObject
{
	public virtual bool OverridesChildren => false;
	public abstract string Highlight(string raw, Token token);
}
}