using System;
using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Targets;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
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
	public Type Type => typeof(Target);
	private readonly ITargetProvider _provider;
	private IReadOnlyList<string> _lastList;
	private HashSet<string> _knownSet = new(StringComparer.OrdinalIgnoreCase);

	public TargetParser() : this(new LiteralTargetProvider())
	{
	}

	public TargetParser(ITargetProvider provider)
	{
		_provider = provider;
	}

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (!raw.StartsWith("@"))
		{
			return TokenState.Failed;
		}

		HashSet<string> known = GetKnownSet();

		if (known.Contains(raw))
		{
			return TokenState.Completed;
		}

		if (known.Any(t => t.StartsWith(raw, StringComparison.OrdinalIgnoreCase)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return new Target(raw);
	}

	private HashSet<string> GetKnownSet()
	{
		IReadOnlyList<string> list = _provider.GetTargets();
		if (!ReferenceEquals(list, _lastList))
		{
			_lastList = list;
			_knownSet = new(list, StringComparer.OrdinalIgnoreCase);
		}

		return _knownSet;
	}
}
}