using System;

public class DoubleParser : ITokenParser
{
	public Type Type => typeof(double);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		return double.TryParse(raw, out _) ? TokenState.Completed : TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return double.Parse(raw);
	}
}