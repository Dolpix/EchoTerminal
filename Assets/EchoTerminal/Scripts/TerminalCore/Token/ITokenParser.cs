using System;

public interface ITokenParser
{
	Type Type { get; }
	TokenState ParseTokenState(string raw, Type expectedType = null);
	object ParseValue(string raw, Type expectedType = null);
}