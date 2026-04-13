using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EchoTerminal.TerminalCore;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
public class TerminalHighlighter : IEchoComponent
{
	private const string _colorWhite = "#FFFFFF";
	private static readonly string ColorCommand = ColorToHex(new(0.40f, 0.78f, 1.00f));
	private static readonly string ColorValid = ColorToHex(new(0.67f, 0.85f, 0.47f));
	private static readonly string ColorInvalid = ColorToHex(new(1.00f, 0.35f, 0.35f));
	private readonly Label _highlightLabel;
	private readonly TextField _inputField;

	private readonly Terminal _terminal;

	public TerminalHighlighter(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_inputField = root?.Q<TextField>("input-field");
		_highlightLabel = root?.Q<Label>("highlight-label");

		if (_inputField == null || _highlightLabel == null)
		{
			return;
		}

		_inputField.RegisterValueChangedCallback(OnInputChanged);
	}

	~TerminalHighlighter()
	{
		_inputField?.UnregisterValueChangedCallback(OnInputChanged);
	}

	private void OnInputChanged(ChangeEvent<string> evt)
	{
		_highlightLabel.text = string.IsNullOrEmpty(evt.newValue)
			? string.Empty
			: BuildHighlightedText(evt.newValue);
	}

	private string BuildHighlightedText(string input)
	{
		var tokenizer = _terminal.Tokenizer;
		var registry = _terminal.Registry;

		var rawTokens = tokenizer.Tokenize(input);
		if (rawTokens.Count == 0)
		{
			return string.Empty;
		}

		var cmdToken = rawTokens[0];
		var cmdState = tokenizer.TryGetParser<CommandName>(out var cmdParser)
			? cmdParser.ParseTokenState(cmdToken.Raw)
			: TokenState.Failed;

		var allTokens = new List<Token> { cmdToken };
		var allColors = new List<string> { TokenStateToCommandColor(cmdState) };

		if (cmdState != TokenState.Completed || !registry.TryGet(cmdToken.Raw, out var entries))
		{
			var rest = tokenizer.Tokenize(input);
			for (var i = 1; i < rest.Count; i++)
			{
				allTokens.Add(rest[i]);
				allColors.Add(ColorInvalid);
			}

			return Colorize(input, allTokens, allColors);
		}

		var argInput = input.TrimStart();
		var spaceAfterCmd = argInput.IndexOf(' ');
		argInput = spaceAfterCmd >= 0 ? argInput[(spaceAfterCmd + 1)..] : string.Empty;

		if (string.IsNullOrWhiteSpace(argInput))
		{
			return Colorize(input, allTokens, allColors);
		}

		List<Token> bestArgTokens = null;
		var bestExpectedCount = -1;
		var bestResolved = -1;
		var bestNonFailed = -1;

		foreach (var entry in entries)
		{
			var expectedTypes = entry.Method.GetParameters().Select(p => p.ParameterType).ToList();
			var argTokens = tokenizer.Tokenize(argInput, expectedTypes);
			var resolved = argTokens.Count(t => t.State == TokenState.Completed);
			var nonFailed = argTokens.Count(t => t.State != TokenState.Failed);

			if (bestArgTokens != null)
			{
				if (resolved < bestResolved)
				{
					continue;
				}

				if (resolved == bestResolved && nonFailed < bestNonFailed)
				{
					continue;
				}

				if (resolved == bestResolved &&
					nonFailed == bestNonFailed &&
					expectedTypes.Count <= bestExpectedCount)
				{
					continue;
				}
			}

			bestArgTokens = argTokens;
			bestExpectedCount = expectedTypes.Count;
			bestResolved = resolved;
			bestNonFailed = nonFailed;
		}

		if (bestArgTokens == null)
		{
			return Colorize(input, allTokens, allColors);
		}

		for (var i = 0; i < bestArgTokens.Count; i++)
		{
			var t = bestArgTokens[i];
			allTokens.Add(t);
			allColors.Add(i >= bestExpectedCount ? ColorInvalid : TokenStateToArgColor(t.State));
		}

		return Colorize(input, allTokens, allColors);
	}

	private static string TokenStateToCommandColor(TokenState state)
	{
		return state switch
		{
			TokenState.Completed => ColorCommand,
			TokenState.Partial   => _colorWhite,
			_                    => ColorInvalid
		};
	}

	private static string TokenStateToArgColor(TokenState state)
	{
		return state switch
		{
			TokenState.Completed => ColorValid,
			TokenState.Partial   => _colorWhite,
			_                    => ColorInvalid
		};
	}

	private static string Colorize(string input, List<Token> tokens, List<string> colors)
	{
		var sb = new StringBuilder();
		var cursor = 0;

		for (var i = 0; i < tokens.Count; i++)
		{
			var raw = tokens[i].Raw;
			var start = input.IndexOf(raw, cursor, StringComparison.Ordinal);
			if (start < 0)
			{
				break;
			}

			if (start > cursor)
			{
				sb.Append(new string(' ', start - cursor));
			}

			sb.Append($"<color={colors[i]}>{EscapeRichText(raw)}</color>");
			cursor = start + raw.Length;
		}

		return sb.ToString();
	}

	private static string EscapeRichText(string raw)
	{
		return raw.Replace("<", "<\u200b");
	}

	private static string ColorToHex(Color c)
	{
		return $"#{Mathf.RoundToInt(c.r * 255):X2}{Mathf.RoundToInt(c.g * 255):X2}{Mathf.RoundToInt(c.b * 255):X2}";
	}
}
}