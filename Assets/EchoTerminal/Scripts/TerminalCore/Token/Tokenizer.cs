using System.Collections.Generic;
using System.Linq;

public static class Tokenizer
{
	public static List<Token> Tokenize(string input, IList<ITokenParser> parsers)
	{
		var tokens = new List<Token>();
		var buf = "";

		for (var i = 0; i < input.Length; i++)
		{
			var ch = input[i];

			if (ch == ' ' && buf.Length > 0)
			{
				// Ask every parser: are you still reading this?
				var anyPending = parsers.Any(p =>
					p.Parse(buf, false) == TokenState.Pending);

				if (anyPending)
				{
					// Someone says "I'm not done" — space is part of the token
					buf += ch;
				}
				else
				{
					// Nobody needs more — finalize and split
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