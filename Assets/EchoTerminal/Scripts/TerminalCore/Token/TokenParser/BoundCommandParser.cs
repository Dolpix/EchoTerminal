using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.TerminalCore
{
public readonly struct BoundCommand
{
	public readonly string Raw;
	public readonly List<Token> Tokens;

	public BoundCommand(string raw, List<Token> tokens)
	{
		Raw = raw;
		Tokens = tokens;
	}

	public override string ToString()
	{
		return Raw;
	}
}

public class BoundCommandParser : ITokenParser
{
	private readonly List<ITokenParser> _parsers;
	private Tokenizer _tokenizer;

	public BoundCommandParser(List<ITokenParser> parsers)
	{
		_parsers = parsers;
	}

	public Type Type => typeof(BoundCommand);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '>')
		{
			return TokenState.Unresolved;
		}

		if (raw[^1] != '<')
		{
			return TokenState.Pending;
		}

		var inner = raw[1..^1];

		if (string.IsNullOrWhiteSpace(inner))
		{
			return TokenState.Invalid;
		}

		var tokens = GetTokenizer().Tokenize(inner);

		if (tokens.Count == 0 || tokens.Any(t => t.State == TokenState.Invalid))
		{
			return TokenState.Invalid;
		}

		return tokens.Any(t => t.State == TokenState.Pending)
			? TokenState.Pending
			: TokenState.Resolved;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var inner = raw[1..^1];
		var tokens = GetTokenizer().Tokenize(inner);
		return new BoundCommand(inner, tokens);
	}

	private Tokenizer GetTokenizer()
	{
		return _tokenizer ??= new(_parsers);
	}
}
}