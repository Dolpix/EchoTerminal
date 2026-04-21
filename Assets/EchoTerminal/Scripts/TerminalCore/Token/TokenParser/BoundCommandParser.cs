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

public class BoundCommandParser : ITokenParser, IRecursiveParser
{
	private readonly List<ITokenParser> _parsers;
	private Func<string, bool> _commandValidator;
	private Func<string, CommandParseResult> _commandParser;
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
			return TokenState.Failed;
		}

		if (raw[^1] != '<')
		{
			var partialInner = raw[1..];
			if (partialInner.Length <= 0)
			{
				return TokenState.Partial;
			}

			var partialTokens = GetTokenizer().Tokenize(partialInner);
			return partialTokens.Any(t => t.State == TokenState.Failed) ? TokenState.Failed : TokenState.Partial;
		}

		var inner = raw[1..^1];

		if (string.IsNullOrWhiteSpace(inner))
		{
			return TokenState.Failed;
		}

		if (_commandValidator != null)
		{
			return _commandValidator(inner) ? TokenState.Completed : TokenState.Failed;
		}

		var tokens = GetTokenizer().Tokenize(inner);

		if (tokens.Count == 0 || tokens.Any(t => t.State == TokenState.Failed) || tokens.Count > 2)
		{
			return TokenState.Failed;
		}

		return tokens.Any(t => t.State == TokenState.Partial)
			? TokenState.Partial
			: TokenState.Completed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var inner = raw[1..^1];
		var tokens = GetTokenizer().Tokenize(inner);
		return new BoundCommand(inner, tokens);
	}

	public void SetCommandValidator(Func<string, bool> validator)
	{
		_commandValidator = validator;
	}

	public void SetCommandParser(Func<string, CommandParseResult> parser)
	{
		_commandParser = parser;
	}

	public List<Token> GetSubTokens(string raw, Type expectedType)
	{
		var outerState = ParseTokenState(raw, expectedType);
		var result = new List<Token>();

		if (raw.Length == 0 || raw[0] != '>')
		{
			result.Add(new Token { Raw = raw, State = outerState, ExpectedType = expectedType });
			return result;
		}

		bool hasClose = raw[^1] == '<';
		string inner = hasClose ? raw[1..^1] : raw[1..];

		var delimState = hasClose ? outerState : TokenState.Partial;
		result.Add(new Token { Raw = ">", State = delimState });

		if (inner.Length > 0)
		{
			List<Token> innerTokens;
			if (_commandParser != null)
			{
				var parseResult = _commandParser(inner);
				var allInner = new List<Token>();
				if (!string.IsNullOrEmpty(parseResult.CommandToken.Raw))
				{
					allInner.Add(parseResult.CommandToken);
				}

				if (parseResult.ArgTokens != null)
				{
					allInner.AddRange(parseResult.ArgTokens);
				}

				innerTokens = allInner;
			}
			else
			{
				innerTokens = GetTokenizer().Tokenize(inner);
			}

			int pos = 0;
			foreach (var token in innerTokens)
			{
				int tokenStart = inner.IndexOf(token.Raw, pos, StringComparison.Ordinal);
				if (tokenStart < 0) { break; }
				if (tokenStart > pos)
				{
					result.Add(new Token { Raw = inner[pos..tokenStart], State = delimState });
				}

				result.Add(token);
				pos = tokenStart + token.Raw.Length;
			}

			if (pos < inner.Length)
			{
				result.Add(new Token { Raw = inner[pos..], State = delimState });
			}
		}

		if (hasClose)
		{
			result.Add(new Token { Raw = "<", State = outerState });
		}

		return result;
	}

	private Tokenizer GetTokenizer()
	{
		return _tokenizer ??= new(_parsers);
	}
}
}