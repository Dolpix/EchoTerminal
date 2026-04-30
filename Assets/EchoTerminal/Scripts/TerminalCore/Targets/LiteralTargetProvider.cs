using System.Collections.Generic;

namespace EchoTerminal.Scripts.TerminalCore.Targets
{
public class LiteralTargetProvider : ITargetProvider
{
	private readonly IReadOnlyList<string> _targets;

	public LiteralTargetProvider(params string[] targets)
	{
		_targets = targets;
	}

	public IReadOnlyList<string> GetTargets()
	{
		return _targets;
	}
}
}