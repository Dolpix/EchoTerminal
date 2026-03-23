using System;
using EchoTerminal;
using UnityEngine;

namespace EchoTerminal.Scripts.Test
{
public class GameObjectParser : IParser, ITokenParser
{
	public Type TargetType => typeof(GameObject);
	public string TypeName => "GameObject";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (string.IsNullOrEmpty(raw) || raw[0] != '@')
		{
			return TokenState.Unresolved;
		}

		var name = raw.Length > 1 ? raw[1..] : "";
		if (name.Length == 0)
		{
			return isFinalized ? TokenState.Invalid : TokenState.Unresolved;
		}

		var found = GameObject.Find(name);
		if (found != null)
		{
			return TokenState.Resolved;
		}

		// Right format (@name), target not found — still typing or genuinely missing
		return isFinalized ? TokenState.Invalid : TokenState.Unresolved;
	}

	public bool TryParse(string input, out object result, out int charsConsumed)
	{
		result = null;
		charsConsumed = 0;

		if (string.IsNullOrEmpty(input) || input[0] != '@')
		{
			return false;
		}

		var space = input.IndexOf(' ', 1);
		var name = space == -1 ? input[1..] : input[1..space];
		charsConsumed = name.Length + 1;
		result = GameObject.Find(name);
		return true;
	}
}
}