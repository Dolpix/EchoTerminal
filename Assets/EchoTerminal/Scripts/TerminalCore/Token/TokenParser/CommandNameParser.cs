using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public readonly struct CommandName
{
	public readonly string Value;

	public CommandName(string value)
	{
		Value = value;
	}

	public override string ToString()
	{
		return Value;
	}
}

public class CommandNameParser : ITokenParser
{
	public Type Type => typeof(CommandName);
	private readonly HashSet<string> _snapshot;
	private readonly Func<IEnumerable<string>> _live;

	private IEnumerable<string> Source => _live != null ? _live() : _snapshot;

	public CommandNameParser(IEnumerable<string> commandNames)
	{
		_snapshot = new(commandNames);
	}

	public CommandNameParser(Func<IEnumerable<string>> commandNames)
	{
		_live = commandNames;
	}

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		IEnumerable<string> source = Source;
		IEnumerable<string> enumerable = source as string[] ?? source.ToArray();
		if (enumerable.Contains(raw))
		{
			return TokenState.Completed;
		}

		if (raw.Length <= 0)
		{
			return TokenState.Failed;
		}

		return enumerable.Any(name => name.StartsWith(raw, StringComparison.Ordinal))
			? TokenState.Partial
			: TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return new CommandName(raw);
	}
}
}