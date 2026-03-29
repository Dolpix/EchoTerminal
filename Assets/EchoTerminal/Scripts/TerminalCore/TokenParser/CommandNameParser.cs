using System.Collections.Generic;

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

	public TokenState Parse(string raw)
	{
		return _registry.Contains(raw) ? TokenState.Resolved : TokenState.Unresolved;
	}
}