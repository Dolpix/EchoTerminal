using System;

public class BoolParser : ITokenParser
{
    public Type Type => typeof(bool);

    public TokenState ParseTokenState(string raw)
    {
        if (bool.TryParse(raw, out _))
        {
            return TokenState.Resolved;
        }

        return TokenState.Unresolved;
    }

    public object ParseValue(string raw)
    {
        return bool.Parse(raw);
    }
}