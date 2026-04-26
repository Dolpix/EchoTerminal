using System.Collections.Generic;

namespace EchoTerminal
{
public interface ITargetProvider
{
	IReadOnlyList<string> GetTargets();
}
}