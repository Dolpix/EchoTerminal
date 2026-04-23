using System;
using UnityEngine;

namespace EchoTerminal
{
[CreateAssetMenu(fileName = "BoolHighlighter", menuName = "Echo Terminal/Highlighters/Bool")]
public class BoolHighlighter : TokenHighlighter
{
	[SerializeField] private Color _trueColor = new(0.2f, 1f, 0.4f);
	[SerializeField] private Color _falseColor = new(1f, 0.33f, 0.33f);

	public override string Highlight(string raw, Token token)
	{
		bool isTrue = raw.Equals("true", StringComparison.OrdinalIgnoreCase);
		Color color = isTrue ? _trueColor : _falseColor;
		return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{raw}</color>";
	}
}
}