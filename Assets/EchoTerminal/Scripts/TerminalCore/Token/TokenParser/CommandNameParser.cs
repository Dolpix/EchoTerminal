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

	public TokenState ParseTokenState(string raw)
	{
		return _registry.Contains(raw) ? TokenState.Resolved : TokenState.Unresolved;
	}

	public object ParseValue(string raw)
	{
		return new CommandName(raw);
	}
}
}