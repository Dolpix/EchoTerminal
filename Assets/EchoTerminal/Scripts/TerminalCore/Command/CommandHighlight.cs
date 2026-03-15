using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EchoTerminal
{
public class CommandHighlight
{
	private readonly TerminalHighlightColors _colors;
	private readonly CommandParser _parser;

	public CommandHighlight(CommandParser parser, TerminalHighlightColors colors)
	{
		_parser = parser;
		_colors = colors;
	}

	public string Highlight(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}

		var result = _parser.Parse(input);

		if (string.IsNullOrEmpty(result.CommandName))
		{
			return input;
		}

		var sb = new StringBuilder();
		sb.Append(' ', result.LeadingSpaces);

		if (_colors != null)
		{
			var cmdColor = ToHex(result.IsKnownCommand ? _colors.CommandColor : _colors.UnknownColor);
			sb.Append($"<color={cmdColor}>{result.CommandName}</color>");
		}
		else
		{
			sb.Append(result.CommandName);
		}

		if (result.Args == null)
		{
			return sb.ToString();
		}

		var hasTarget = result.Overloads.Any(o => o.Params.Count > 0 && o.Params[0].Expected.IsTarget);
		var pos = result.LeadingSpaces + result.CommandName.Length;
		var paramIndex = 0;
		var targetConsumed = false;

		while (pos < input.Length)
		{
			if (input[pos] == ' ')
			{
				sb.Append(' ');
				pos++;
				continue;
			}

			var end = input.IndexOf(' ', pos);
			if (end == -1)
			{
				end = input.Length;
			}

			var token = input[pos..end];

			if (hasTarget && !targetConsumed && token.StartsWith("@"))
			{
				targetConsumed = true;
				sb.Append(ColorizeTyped(token, typeof(GameObject)));
			}
			else
			{
				sb.Append(ColorizeAtPosition(result.Overloads, paramIndex, token, targetConsumed));
				paramIndex++;
			}

			pos = end;
		}

		return sb.ToString();
	}

	private string ColorizeAtPosition(
		IReadOnlyList<OverloadResult> overloads,
		int paramIndex,
		string token,
		bool targetConsumed)
	{
		if (_colors == null)
		{
			return token;
		}

		if (overloads.Count == 0)
		{
			return $"<color={ToHex(_colors.FallbackParamColor)}>{token}</color>";
		}

		foreach (var overload in overloads)
		{
			var offset = targetConsumed && overload.Params.Count > 0 && overload.Params[0].Expected.IsTarget ? 1 : 0;
			var idx = paramIndex + offset;

			if (idx >= overload.Params.Count)
			{
				continue;
			}

			if (overload.Params[idx].IsValid)
			{
				return ColorizeTyped(token, overload.Params[idx].Expected.Type);
			}
		}

		return $"<color={ToHex(_colors.UnknownColor)}>{token}</color>";
	}

	private string ColorizeTyped(string token, Type colorType)
	{
		if (_colors == null)
		{
			return token;
		}

		var color = colorType != null && _colors.TypeColors.TryGetValue(colorType, out var c)
			? c
			: _colors.FallbackParamColor;

		return $"<color={ToHex(color)}>{token}</color>";
	}

	private static string ToHex(Color c)
	{
		return "#" + ColorUtility.ToHtmlStringRGB(c);
	}
}
}