using System.Linq;
using UnityEngine;

namespace EchoTerminal
{
[CreateAssetMenu(fileName = "ColorTypeHighlighter", menuName = "Echo Terminal/Highlighters/Color Type")]
public class ColorTypeHighlighter : TokenHighlighter
{
	private static readonly string[] NamedColors =
	{
		"red", "green", "blue", "white", "black",
		"yellow", "cyan", "magenta", "clear", "grey", "gray"
	};

	[SerializeField] private Color _fallback = new(0.33f, 1f, 0.53f);

	public override string Highlight(string raw, Token token)
	{
		Color color = TryParseColor(raw, out Color parsed) ? parsed : _fallback;
		return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{raw}</color>";
	}

	private static bool TryParseColor(string raw, out Color color)
	{
		if (raw.StartsWith("#") && ColorUtility.TryParseHtmlString(raw, out color))
		{
			return true;
		}

		if (raw.StartsWith("(") && TryParseTuple(raw, out color))
		{
			return true;
		}

		string lower = raw.ToLowerInvariant();
		if (NamedColors.Contains(lower) && ColorUtility.TryParseHtmlString(lower, out color))
		{
			return true;
		}

		color = default;
		return false;
	}

	private static bool TryParseTuple(string raw, out Color color)
	{
		color = default;
		string inner = raw.Trim('(', ')');
		string[] parts = inner.Split(',');
		if (parts.Length is not (3 or 4))
		{
			return false;
		}

		if (!float.TryParse(parts[0].Trim(), out float r) ||
			!float.TryParse(parts[1].Trim(), out float g) ||
			!float.TryParse(parts[2].Trim(), out float b))
		{
			return false;
		}

		float a = parts.Length == 4 && float.TryParse(parts[3].Trim(), out float pa) ? pa : 1f;
		color = new(r, g, b, a);
		return true;
	}
}
}