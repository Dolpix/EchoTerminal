using System.Text;
using EchoTerminal.Scripts.TerminalCore.Token;
using UnityEngine;

namespace EchoTerminal.Scripts.ScriptableObjects.Highlighting.Highlighters
{
[CreateAssetMenu(fileName = "RainbowHighlighter", menuName = "Echo Terminal/Highlighters/Rainbow")]
public class RainbowHighlighter : TokenHighlighter
{
	[SerializeField] [Range(0f, 1f)] private float _hueOffset;
	[SerializeField] [Range(0.01f, 0.5f)] private float _hueStep = 0.08f;
	[SerializeField] [Range(0f, 1f)] private float _saturation = 1f;
	[SerializeField] [Range(0f, 1f)] private float _value = 1f;

	public override string Highlight(string raw, Token token)
	{
		var sb = new StringBuilder(raw.Length * 24);
		for (var i = 0; i < raw.Length; i++)
		{
			float hue = (_hueOffset + i * _hueStep) % 1f;
			Color color = Color.HSVToRGB(hue, _saturation, _value);
			sb.Append("<color=#");
			sb.Append(ColorUtility.ToHtmlStringRGB(color));
			sb.Append('>');
			sb.Append(raw[i]);
			sb.Append("</color>");
		}

		return sb.ToString();
	}
}
}