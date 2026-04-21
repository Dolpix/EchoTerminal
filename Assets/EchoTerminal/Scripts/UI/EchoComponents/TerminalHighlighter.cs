using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
public class TerminalHighlighter : IEchoComponent
{
	private const string _colorValidCommand = "#5599FF";
	private const string _colorValidParameter = "#55FF88";
	private const string _colorInprogress = "#FFFFFF";
	private const string _colorInvalid = "#FF5555";

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
		_highlightLabel.text = string.IsNullOrEmpty(evt.newValue) ? string.Empty : BuildHighlightedText(evt.newValue);
	}

	private string BuildHighlightedText(string input)
	{
		var result = _terminal.CommandParser.Parse(input);
		var tokens = new List<(string raw, string color)>();

		if (!string.IsNullOrEmpty(result.CommandToken.Raw))
		{
			var commandColor = result.Entries == null
				? result.CommandToken.State == TokenState.Partial ? _colorInprogress : _colorInvalid
				: _colorValidCommand;

			tokens.Add((result.CommandToken.Raw, commandColor));
		}

		foreach (var token in result.ArgTokens ?? new())
		{
			tokens.AddRange(ExpandToken(token));
		}

		return ApplyColors(input, tokens);
	}

	private List<(string raw, string color)> ExpandToken(Token token)
	{
		var resolvedType = token.Type ?? token.ExpectedType;
		if (resolvedType == null ||
		    !_terminal.Tokenizer.TryGetParser(resolvedType, out var parser) ||
		    parser is not IRecursiveParser recursive)
		{
			return new List<(string, string)> { (token.Raw, ColorForState(token.State)) };
		}

		var subs = recursive.GetSubTokens(token.Raw, token.ExpectedType);
		if (subs.Count > 1 || (subs.Count == 1 && subs[0].Raw != token.Raw))
		{
			var spans = new List<(string raw, string color)>();
			foreach (var sub in subs)
			{
				spans.AddRange(ExpandToken(sub));
			}

			return spans;
		}

		return new List<(string, string)> { (token.Raw, ColorForState(token.State)) };
	}

	private string ColorForState(TokenState state) => state switch
	{
		TokenState.Completed => _colorValidParameter,
		TokenState.Partial   => _colorInprogress,
		_                    => _colorInvalid
	};

	private static string ApplyColors(string input, List<(string raw, string color)> tokens)
	{
		var sb = new StringBuilder();
		var pos = 0;

		foreach (var (raw, color) in tokens)
		{
			var tokenStart = input.IndexOf(raw, pos, StringComparison.Ordinal);
			if (tokenStart < 0)
			{
				break;
			}

			sb.Append(input[pos..tokenStart]);
			sb.Append($"<color={color}>{raw}</color>");
			pos = tokenStart + raw.Length;
		}

		sb.Append(input[pos..]);
		return sb.ToString();
	}
}
}