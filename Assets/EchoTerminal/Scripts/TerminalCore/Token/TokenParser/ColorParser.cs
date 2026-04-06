using System;
using System.Linq;
using UnityEngine;

public class ColorParser : ITokenParser
{
	private static readonly string[] NamedColors =
	{
		"red", "green", "blue", "white", "black",
		"yellow", "cyan", "magenta", "clear", "grey", "gray"
	};

	public Type Type => typeof(Color);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Unresolved;
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

		if (ColorUtility.TryParseHtmlString(raw, out var color))
		{
			return color;
		}

		return null;
	}

	private static TokenState ParseTupleState(string raw)
	{
		if (!raw.EndsWith(")"))
		{
			return TokenState.Pending;
		}

		var parts = raw[1..^1].Split(',');
		if (parts.Length is not (3 or 4))
		{
			return TokenState.Invalid;
		}

		foreach (var part in parts)
		{
			if (!float.TryParse(part.Trim(), out _))
			{
				return TokenState.Invalid;
			}
		}

		return TokenState.Resolved;
	}

	private static TokenState ParseHexState(string raw)
	{
		var hex = raw[1..];

		return hex.Length switch
		{
			6 or 8   => hex.All(Uri.IsHexDigit) ? TokenState.Resolved : TokenState.Invalid,
			< 6 or 7 => hex.All(Uri.IsHexDigit) ? TokenState.Pending : TokenState.Invalid,
			_        => TokenState.Invalid
		};
	}

	private static TokenState ParseNamedState(string raw)
	{
		var lower = raw.ToLowerInvariant();

		if (NamedColors.Contains(lower))
		{
			return TokenState.Resolved;
		}

		if (NamedColors.Any(n => n.Length > lower.Length && n.StartsWith(lower)))
		{
			return TokenState.Pending;
		}

		return TokenState.Unresolved;
	}

	private static Color ParseTupleValue(string raw)
	{
		var parts = raw[1..^1].Split(',');
		var r = float.Parse(parts[0].Trim());
		var g = float.Parse(parts[1].Trim());
		var b = float.Parse(parts[2].Trim());
		var a = parts.Length == 4 ? float.Parse(parts[3].Trim()) : 1f;
		return new(r, g, b, a);
	}
}