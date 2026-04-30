using System.Collections.Generic;

namespace EchoTerminal.Scripts.TerminalCore.Targets
{
public interface ITargetProvider
{
	IReadOnlyList<string> GetTargets();
}
}