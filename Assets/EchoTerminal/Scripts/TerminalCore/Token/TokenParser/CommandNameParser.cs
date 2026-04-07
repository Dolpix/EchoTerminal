using System.Collections.Generic;

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

	public System.Type Type => typeof(CommandName);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (_registry.Contains(raw))
		{
			return TokenState.Resolved;
		}

		foreach (var name in _registry)
		{
			if (name.StartsWith(raw, System.StringComparison.OrdinalIgnoreCase))
			{
				return TokenState.Unresolved;
			}
		}

		return TokenState.Invalid;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return new CommandName(raw);
	}
}
}