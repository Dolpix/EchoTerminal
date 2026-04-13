using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoTerminal.TerminalCore
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
	private readonly HashSet<string> _registry;

	public CommandNameParser(IEnumerable<string> commandNames)
	{
		_registry = new(commandNames);
	}

	public Type Type => typeof(CommandName);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (_registry.Contains(raw))
		{
			return TokenState.Completed;
		}

		if (raw.Length <= 0)
		{
			return TokenState.Failed;
		}

		return _registry.Any(name => name.StartsWith(raw, StringComparison.Ordinal))
			? TokenState.Partial
			: TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return new CommandName(raw);
	}
}
}