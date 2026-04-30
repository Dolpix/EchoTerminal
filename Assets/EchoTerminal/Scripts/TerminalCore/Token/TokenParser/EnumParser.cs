using System;
using System.Linq;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class EnumParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(System.Enum);
	public string HintLabel => "value";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || expectedType is not { IsEnum: true })
		{
			return TokenState.Failed;
		}

		string[] names = System.Enum.GetNames(expectedType);

		if (names.Any(n => string.Equals(n, raw, StringComparison.OrdinalIgnoreCase)))
		{
			return TokenState.Completed;
		}

		string lower = raw.ToLowerInvariant();
		return names.Any(n => n.Length > raw.Length && n.ToLowerInvariant().StartsWith(lower))
			? TokenState.Partial
			: TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return expectedType is not { IsEnum: true } ? null : System.Enum.Parse(expectedType, raw, true);
	}
}
}