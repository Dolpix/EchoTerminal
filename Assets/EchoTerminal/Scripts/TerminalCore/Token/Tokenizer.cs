using System;
using System.Collections.Generic;

namespace EchoTerminal.TerminalCore
{
    public class Tokenizer
    {
        private readonly List<ITokenParser> _parsers;
        private readonly Dictionary<Type, ITokenParser> _parsersByType;

        public Tokenizer(List<ITokenParser> parsers)
        {
            _parsers = parsers;
            _parsersByType = new Dictionary<Type, ITokenParser>();
            foreach (var p in parsers)
            {
                _parsersByType[p.Type] = p;
            }
        }

        public List<Token> Tokenize(string input, List<Type> expectedTypes = null)
        {
            var tokens = new List<Token>();

            if (string.IsNullOrWhiteSpace(input))
            {
                return tokens;
            }

            var pos = input.Length - input.TrimStart().Length;

            while (pos < input.Length)
            {
                while (pos < input.Length && input[pos] == ' ')
                    pos++;

                if (pos >= input.Length)
                {
                    break;
                }

                var tokenIndex = tokens.Count;
                var expectedType = expectedTypes != null && tokenIndex < expectedTypes.Count
                    ? expectedTypes[tokenIndex]
                    : null;

                var start = pos;
                pos++;
                var raw = input[start..pos];

                var pendingParser = FindPendingParser(raw, expectedType);

                if (pendingParser != null)
                {
                    while (pos < input.Length)
                    {
                        pos++;
                        raw = input[start..pos];
                        if (pendingParser.ParseTokenState(raw) != TokenState.Pending)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    while (pos < input.Length && input[pos] != ' ')
                        pos++;
                }

                raw = input[start..pos];
                tokens.Add(BuildToken(raw, expectedType));
            }

            return tokens;
        }

        private ITokenParser FindPendingParser(string raw, Type expectedType)
        {
            if (expectedType != null && _parsersByType.TryGetValue(expectedType, out var expected))
            {
                if (expected.ParseTokenState(raw) == TokenState.Pending)
                {
                    return expected;
                }
            }

            foreach (var p in _parsers)
            {
                if (p.ParseTokenState(raw) == TokenState.Pending)
                {
                    return p;
                }
            }

            return null;
        }

        private Token BuildToken(string raw, Type expectedType)
        {
            if (expectedType != null && _parsersByType.TryGetValue(expectedType, out var expected))
            {
                var state = expected.ParseTokenState(raw);
                if (state == TokenState.Unresolved)
                {
                    state = TokenState.Invalid;
                }

                return new Token
                {
                    Raw = raw,
                    State = state,
                    Type = state == TokenState.Invalid ? null : expectedType,
                    ExpectedType = expectedType
                };
            }

            foreach (var p in _parsers)
            {
                if (p.ParseTokenState(raw) == TokenState.Resolved)
                {
                    return new Token { Raw = raw, State = TokenState.Resolved, Type = p.Type };
                }
            }

            foreach (var p in _parsers)
            {
                if (p.ParseTokenState(raw) == TokenState.Pending)
                {
                    return new Token { Raw = raw, State = TokenState.Pending, Type = p.Type };
                }
            }

            foreach (var p in _parsers)
            {
                if (p.ParseTokenState(raw) == TokenState.Invalid)
                {
                    return new Token { Raw = raw, State = TokenState.Invalid, Type = p.Type };
                }
            }

            return new Token { Raw = raw, State = TokenState.Unresolved, Type = typeof(string) };
        }
    }
}