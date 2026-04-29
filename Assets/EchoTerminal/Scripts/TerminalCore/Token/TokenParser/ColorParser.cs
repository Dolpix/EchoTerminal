using System;
using System.Linq;
using UnityEngine;

public class ColorParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(Color);
	public string HintLabel => "(r, g, b) | #RRGGBB | name";

	private static readonly string[] NamedColors =
	{
		"red", "green", "blue", "white", "black",
		"yellow", "cyan", "magenta", "clear", "grey", "gray"
	};

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Failed;
		}

		return raw[0] switch
		{
			'(' => ParseTupleState(raw),
			'#' => ParseHexState(raw),
			_   => ParseNamedState(raw)
		};
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return null;
		}

		if (raw[0] == '(')
		{
			return ParseTupleValue(raw);
		}

		if (ParseTokenState(raw) != TokenState.Completed)
		{
			return null;
		}

		if (ColorUtility.TryParseHtmlString(raw, out Color color))
		{
			return color;
		}

		return null;
	}

	private static TokenState ParseTupleState(string raw)
	{
		if (!raw.EndsWith(")"))
		{
			string inner = raw[1..];
			int commaIndex = inner.IndexOf(',');
			var commaCount = 0;
			while (commaIndex >= 0)
			{
				commaCount++;
				if (commaCount >= 4)
				{
					return TokenState.Failed;
				}

				string component = inner[..commaIndex].Trim();
				if (!IsValidFloatComponent(component))
				{
					return TokenState.Failed;
				}

				inner = inner[(commaIndex + 1)..];
				commaIndex = inner.IndexOf(',');
			}

			return !IsValidFloatComponent(inner.Trim()) ? TokenState.Failed : TokenState.Partial;
		}

		string[] parts = raw[1..^1].Split(',');
		if (parts.Length is not (3 or 4))
		{
			return TokenState.Failed;
		}

		foreach (string part in parts)
		{
			if (!float.TryParse(part.Trim(), out _))
			{
				return TokenState.Failed;
			}
		}

		return TokenState.Completed;
	}

	private static TokenState ParseHexState(string raw)
	{
		string hex = raw[1..];

		return hex.Length switch
		{
			6 or 8   => hex.All(Uri.IsHexDigit) ? TokenState.Completed : TokenState.Failed,
			< 6 or 7 => hex.All(Uri.IsHexDigit) ? TokenState.Partial : TokenState.Failed,
			_        => TokenState.Failed
		};
	}

	private static TokenState ParseNamedState(string raw)
	{
		string lower = raw.ToLowerInvariant();

		if (NamedColors.Contains(lower))
		{
			return TokenState.Completed;
		}

		if (NamedColors.Any(n => n.Length > lower.Length && n.StartsWith(lower)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	private static bool IsValidFloatComponent(string s)
	{
		if (s.Length == 0)
		{
			return true;
		}

		return float.TryParse(s, out _) || s.All(c => c == '-' || c == '+' || c == '.' || char.IsDigit(c));
	}

	private static Color ParseTupleValue(string raw)
	{
		string[] parts = raw[1..^1].Split(',');
		float r = float.Parse(parts[0].Trim());
		float g = float.Parse(parts[1].Trim());
		float b = float.Parse(parts[2].Trim());
		float a = parts.Length == 4 ? float.Parse(parts[3].Trim()) : 1f;
		return new(r, g, b, a);
	}
}