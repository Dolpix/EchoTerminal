using System;
using System.Collections.Generic;
using System.Text;
using EchoTerminal.TerminalCore;

namespace EchoTerminal.Components
{
public class TerminalHighlighterCore
{
	public HighlighterSet HighlighterSet { get; }

	private const string _colorInprogress = "#FFFFFF";
	private const string _colorInvalid = "#FF5555";
	private const string _colorValidCommand = "#5599FF";
	private const string _colorDefaultParameter = "#55FF88";
	private const string _colorStructural = "#AAAAAA";

	private readonly CommandParser _commandParser;
	private readonly Tokenizer _tokenizer;

	public TerminalHighlighterCore(CommandParser commandParser, Tokenizer tokenizer, HighlighterSet highlighterSet)
	{
		_commandParser = commandParser;
		_tokenizer = tokenizer;
		HighlighterSet = highlighterSet;
	}

	public string BuildHighlightedText(string input)
	{
		CommandParseResult result = _commandParser.Parse(input);
		var spans = new List<(int start, int length, string richText)>();

		if (!string.IsNullOrEmpty(result.CommandToken.Raw))
		{
			Token cmdToken = result.CommandToken;
			string richText;

			if (cmdToken.State == TokenState.Completed &&
				result.Entries != null &&
				HighlighterSet != null &&
				HighlighterSet.TryGet(typeof(CommandName), out TokenHighlighter cmdHighlighter))
			{
				richText = cmdHighlighter.Highlight(cmdToken.Raw, cmdToken);
			}
			else
			{
				string color = result.Entries == null
					? cmdToken.State == TokenState.Partial ? _colorInprogress : _colorInvalid
					: _colorValidCommand;
				richText = $"<color={color}>{cmdToken.Raw}</color>";
			}

			int cmdStart = input.IndexOf(cmdToken.Raw, StringComparison.Ordinal);
			if (cmdStart >= 0)
			{
				spans.Add((cmdStart, cmdToken.Raw.Length, richText));
			}
		}

		int searchFrom = spans.Count > 0 ? spans[0].start + spans[0].length : 0;
		foreach (Token token in result.ArgTokens ?? new List<Token>())
		{
			int tokenStart = input.IndexOf(token.Raw, searchFrom, StringComparison.Ordinal);
			if (tokenStart < 0)
			{
				continue;
			}

			ExpandToken(token, input, tokenStart, spans);
			searchFrom = tokenStart + token.Raw.Length;
		}

		return ApplySpans(input, spans);
	}

	private void ExpandToken(
		Token token,
		string input,
		int offset,
		List<(int start, int length, string richText)> spans)
	{
		TokenHighlighter highlighter = null;
		if (token.State == TokenState.Completed && token.ExpectedType != null)
		{
			HighlighterSet?.TryGet(token.ExpectedType, out highlighter);
		}

		if (highlighter != null && highlighter.OverridesChildren)
		{
			spans.Add((offset, token.Raw.Length, highlighter.Highlight(token.Raw, token)));
			return;
		}

		if (token.ExpectedType != null &&
			_tokenizer.TryGetParser(token.ExpectedType, out ITokenParser parser) &&
			parser is IRecursiveParser recursive)
		{
			List<Token> subs = recursive.GetSubTokens(token.Raw, token.ExpectedType);
			if (subs.Count > 1 || (subs.Count == 1 && subs[0].Raw != token.Raw))
			{
				int subOffset = offset;
				foreach (Token sub in subs)
				{
					int subStart = token.Raw.IndexOf(sub.Raw, subOffset - offset, StringComparison.Ordinal);
					if (subStart < 0)
					{
						continue;
					}

					ExpandToken(sub, input, offset + subStart, spans);
					subOffset = offset + subStart + sub.Raw.Length;
				}

				return;
			}
		}

		string richText;
		if (highlighter != null)
		{
			richText = highlighter.Highlight(token.Raw, token);
		}
		else if (token.ExpectedType == typeof(CommandName))
		{
			string color = token.State switch
			{
				TokenState.Completed => _colorValidCommand,
				TokenState.Partial   => _colorInprogress,
				_                    => _colorInvalid
			};
			richText = $"<color={color}>{token.Raw}</color>";
		}
		else if (token.ExpectedType == null)
		{
			string color = token.State == TokenState.Partial ? _colorInprogress : _colorStructural;
			richText = $"<color={color}>{token.Raw}</color>";
		}
		else
		{
			richText = RichTextForState(token.Raw, token.State);
		}

		spans.Add((offset, token.Raw.Length, richText));
	}

	private static string RichTextForState(string raw, TokenState state)
	{
		string color = state switch
		{
			TokenState.Completed => _colorDefaultParameter,
			TokenState.Partial   => _colorInprogress,
			_                    => _colorInvalid
		};
		return $"<color={color}>{raw}</color>";
	}

	private static string ApplySpans(string input, List<(int start, int length, string richText)> spans)
	{
		var sb = new StringBuilder(input.Length * 2);
		var pos = 0;

		foreach ((int start, int length, string richText) in spans)
		{
			if (start < pos || start + length > input.Length)
			{
				continue;
			}

			sb.Append(input[pos..start]);
			sb.Append(richText);
			pos = start + length;
		}

		sb.Append(input[pos..]);
		return sb.ToString();
	}
}
}