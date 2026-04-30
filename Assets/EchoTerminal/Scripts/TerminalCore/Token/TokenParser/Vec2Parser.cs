using System;
using System.Linq;
using UnityEngine;

namespace EchoTerminal.Scripts.TerminalCore.Token.TokenParser
{
public class Vec2Parser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(Vector2);
	public string HintLabel => "(x, y)";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '(')
		{
			return TokenState.Failed;
		}

		if (!raw.EndsWith(")"))
		{
			string inner = raw[1..];
			int commaIndex = inner.IndexOf(',');
			var commaCount = 0;
			while (commaIndex >= 0)
			{
				commaCount++;
				if (commaCount >= 2)
				{
					return TokenState.Failed;
				}

				string component = inner[..commaIndex].Trim();
				if (!IsValidFloatComponent(component))
				{
					return TokenState.Failed;
				}

				inner = inner[(commaIndex + 1)..];
				commaIndex = inner.IndexOf(',');
			}

			return !IsValidFloatComponent(inner.Trim()) ? TokenState.Failed : TokenState.Partial;
		}

		string[] parts = raw[1..^1].Split(',');
		if (parts.Length != 2 || parts.Any(part => !float.TryParse(part.Trim(), out _)))
		{
			return TokenState.Failed;
		}

		return TokenState.Completed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		string[] parts = raw[1..^1].Split(',');
		return new Vector2(
			float.Parse(parts[0].Trim()),
			float.Parse(parts[1].Trim())
		);
	}

	private static bool IsValidFloatComponent(string s)
	{
		if (s.Length == 0)
		{
			return true;
		}

		return float.TryParse(s, out _) || s.All(c => c == '-' || c == '+' || c == '.' || char.IsDigit(c));
	}
}
}