using System.Collections.Generic;

namespace EchoTerminal
{
public struct OverloadResult
{
	public CommandEntry Entry { get; }
	public bool IsComplete { get; }
	public IReadOnlyList<ParamResult> Params { get; }

	public OverloadResult(CommandEntry entry, IReadOnlyList<ParamResult> paramResults, bool isComplete)
	{
		Entry = entry;
		Params = paramResults;
		IsComplete = isComplete;
	}
}
}