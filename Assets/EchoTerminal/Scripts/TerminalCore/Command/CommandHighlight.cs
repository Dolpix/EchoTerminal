using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace EchoTerminal
{
public class CommandHighlight
{
	private readonly TerminalHighlightColors _colors;
	private readonly CommandRegistry _registry;

	public CommandHighlight(CommandRegistry registry, TerminalHighlightColors colors)
	{
		_registry = registry;
		_colors = colors;
	}

	public string Highlight(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}

		var trimmed = input.TrimStart();
		var leadingSpaces = input.Length - trimmed.Length;
		var space = trimmed.IndexOf(' ');
		var commandName = space == -1 ? trimmed : trimmed[..space];

		var sb = new StringBuilder();
		sb.Append(' ', leadingSpaces);

		if (_colors != null)
		{
			var isKnown = _registry.TryGet(commandName, out _);
			var cmdColor = ToHex(isKnown ? _colors.CommandColor : _colors.UnknownCommandColor);
			sb.Append($"<color={cmdColor}>{commandName}</color>");
		}
		else
		{
			sb.Append(commandName);
		}

		if (space == -1)
		{
			return sb.ToString();
		}

		var pos = leadingSpaces + space;
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
			sb.Append(ColorizeToken(token));
			pos = end;
		}

		return sb.ToString();
	}

	private string ColorizeToken(string token)
	{
		if (_colors == null)
		{
			return token;
		}

		Type type;

		if (token.StartsWith("@"))
		{
			type = typeof(GameObject);
		}
		else if (bool.TryParse(token, out _))
		{
			type = typeof(bool);
		}
		else if (token.Contains(','))
		{
			type = typeof(Vector3);
		}
		else if (float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
		{
			type = typeof(float);
		}
		else
		{
			type = typeof(string);
		}

		var color = _colors.TypeColors.TryGetValue(type, out var c) ? c : _colors.FallbackParamColor;
		return $"<color={ToHex(color)}>{token}</color>";
	}

	private static string ToHex(Color c)
	{
		return "#" + ColorUtility.ToHtmlStringRGB(c);
	}
}
}