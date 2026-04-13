using System.Collections.Generic;
using System.Linq;

public readonly struct Target
{
	public readonly string Value;

	public Target(string value)
	{
		Value = value;
	}

	public override string ToString()
	{
		return Value;
	}
}

public class TargetParser : ITokenParser
{
	private readonly HashSet<string> _known;

	public TargetParser(IEnumerable<string> knownTargets)
	{
		_known = new(knownTargets);
	}

	public System.Type Type => typeof(Target);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (!raw.StartsWith("@"))
		{
			return TokenState.Failed;
		}

		if (_known.Contains(raw))
		{
			return TokenState.Completed;
		}

		if (_known.Any(t => t.StartsWith(raw)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return new Target(raw);
	}
}