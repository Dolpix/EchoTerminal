using UnityEngine;

namespace EchoTerminal
{
public abstract class TokenHighlighter : ScriptableObject
{
	// When true, recursive parsers will not expand children — this highlighter handles the full raw string.
	// When false (default), children use their own highlighters; this highlighter handles leaf tokens only.
	public virtual bool OverridesChildren => false;

	// Returns rich text for a completed token. Only called when TokenState == Completed.
	public abstract string Highlight(string raw, Token token);
}
}
