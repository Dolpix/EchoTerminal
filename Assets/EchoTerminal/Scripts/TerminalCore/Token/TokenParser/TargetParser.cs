using System.Collections.Generic;
using System.Linq;

public class TargetParser : ITokenParser
{
	private readonly HashSet<string> _known;

	public TargetParser(IEnumerable<string> knownTargets)
	{
		_known = new(knownTargets);
	}

	public System.Type Type => typeof(string);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (!raw.StartsWith("@"))
		{
			return TokenState.Unresolved;
		}

		if (_known.Contains(raw))
		{
			return TokenState.Resolved;
		}

		if (_known.Any(t => t.StartsWith(raw)))
		{
			return TokenState.Pending;
		}

		return TokenState.Invalid;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return raw;
	}
}