using System;
using System.Collections.Generic;
using System.Text;
using EchoTerminal.TerminalCore;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
    public class TerminalHighlighter : IEchoComponent
    {
        private const string ColorWhite = "#FFFFFF";
        private static readonly string ColorCommand = ColorToHex(new Color(0.40f, 0.78f, 1.00f));
        private static readonly string ColorValid = ColorToHex(new Color(0.67f, 0.85f, 0.47f));
        private static readonly string ColorInvalid = ColorToHex(new Color(1.00f, 0.35f, 0.35f));
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

            // Tokenize without type hints so the tokenizer splits words correctly
            var rawTokens = tokenizer.Tokenize(input);
            if (rawTokens.Count == 0)
            {
                return string.Empty;
            }

            // Ask CommandNameParser directly for the raw state — bypasses BuildToken's Unresolved→Invalid promotion
            var cmdToken = rawTokens[0];
            var cmdState = tokenizer.TryGetParser<CommandName>(out var cmdParser)
                ? cmdParser.ParseTokenState(cmdToken.Raw)
                : TokenState.Invalid;

            var allTokens = new List<Token> { cmdToken };
            var allColors = new List<string> { TokenStateToCommandColor(cmdState) };

            // If command isn't resolved, tokenize the rest without type hints so they stay visible
            if (cmdState != TokenState.Resolved || !registry.TryGet(cmdToken.Raw, out var entries))
            {
                var rest = tokenizer.Tokenize(input);
                for (var i = 1; i < rest.Count; i++)
                {
                    allTokens.Add(rest[i]);
                    allColors.Add(ColorInvalid);
                }

                return Colorize(input, allTokens, allColors);
            }

            // Slice arg input
            var argInput = input.TrimStart();
            var spaceAfterCmd = argInput.IndexOf(' ');
            argInput = spaceAfterCmd >= 0 ? argInput[(spaceAfterCmd + 1)..] : string.Empty;

            if (string.IsNullOrWhiteSpace(argInput))
            {
                return Colorize(input, allTokens, allColors);
            }

            // Try each overload, pick the one that resolves the most tokens
            List<Token> bestArgTokens = null;
            var bestExpectedCount = -1;
            var bestResolved = -1;

            foreach (var entry in entries)
            {
                var expectedTypes = new List<Type>();
                foreach (var p in entry.Method.GetParameters())
                {
                    expectedTypes.Add(p.ParameterType);
                }

                var argTokens = tokenizer.Tokenize(argInput, expectedTypes);
                var resolved = 0;
                foreach (var t in argTokens)
                {
                    if (t.State == TokenState.Resolved)
                    {
                        resolved++;
                    }
                }

                if (bestArgTokens == null
                    || resolved > bestResolved
                    || (resolved == bestResolved && expectedTypes.Count > bestExpectedCount))
                {
                    bestArgTokens = argTokens;
                    bestExpectedCount = expectedTypes.Count;
                    bestResolved = resolved;
                }
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
                TokenState.Resolved => ColorCommand,
                TokenState.Unresolved => ColorWhite,
                TokenState.Pending => ColorWhite,
                _ => ColorInvalid
            };
        }

        private static string TokenStateToArgColor(TokenState state)
        {
            return state switch
            {
                TokenState.Resolved => ColorValid,
                TokenState.Unresolved => ColorWhite,
                TokenState.Pending => ColorWhite,
                _ => ColorInvalid
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