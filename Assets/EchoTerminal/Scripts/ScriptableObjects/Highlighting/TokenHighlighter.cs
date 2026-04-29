using UnityEngine;

namespace EchoTerminal
{
public abstract class TokenHighlighter : ScriptableObject
{
	public virtual bool OverridesChildren => false;
	public abstract string Highlight(string raw, Token token);
}
}