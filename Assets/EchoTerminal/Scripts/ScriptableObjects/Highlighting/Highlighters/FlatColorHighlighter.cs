using UnityEngine;

namespace EchoTerminal
{
[CreateAssetMenu(fileName = "FlatColorHighlighter", menuName = "Echo Terminal/Highlighters/Flat Color")]
public class FlatColorHighlighter : TokenHighlighter
{
	[SerializeField] private Color _color = new(0.33f, 1f, 0.53f);

	public void SetColor(Color color)
	{
		_color = color;
	}

	public override string Highlight(string raw, Token token)
	{
		return $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{raw}</color>";
	}
}
}