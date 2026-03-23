using System.Collections.Generic;

namespace EchoTerminal
{
public class Token
{
	public string Raw;
	public bool IsFinalized;
}

/// <summary>
/// Layer 2 — walks the input character by character, accumulating a buffer.
/// On every space, asks all registered <see cref="ITokenParser"/>s whether any return
/// <see cref="TokenState.Pending"/>. If so, the space becomes part of the token.
/// If not, the buffer is finalized and a new token begins.
/// Parsers — not the tokenizer — define what a token looks like.
/// </summary>
public static class Tokenizer
{
	public static List<Token> Tokenize(string input, IReadOnlyList<ITokenParser> parsers)
	{
		var tokens = new List<Token>();
		var buf = "";

		for (var i = 0; i < input.Length; i++)
		{
			var ch = input[i];

			if (ch == ' ' && buf.Length > 0)
			{
				var anyPending = false;
				foreach (var p in parsers)
				{
					if (p.Parse(buf, false) == TokenState.Pending)
					{
						anyPending = true;
						break;
					}
				}

				if (anyPending)
				{
					buf += ch;
				}
				else
				{
					tokens.Add(new() { Raw = buf, IsFinalized = true });
					buf = "";
				}
			}
			else if (ch != ' ' || buf.Length > 0)
			{
				buf += ch;
			}
		}

		if (buf.Length > 0)
		{
			tokens.Add(new() { Raw = buf, IsFinalized = false });
		}

		return tokens;
	}
}
}
