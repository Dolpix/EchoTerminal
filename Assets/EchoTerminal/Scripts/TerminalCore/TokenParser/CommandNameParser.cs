using System.Collections.Generic;

public class CommandNameParser : ITokenParser
{
	private readonly HashSet<string> _registry;

	public CommandNameParser(IEnumerable<string> commandNames)
	{
		_registry = new(commandNames);
	}

	public string Name => "Command";

	public TokenState Parse(string raw, bool isFinalized)
	{
		return _registry.Contains(raw) ? TokenState.Resolved : TokenState.Unresolved;
	}
}