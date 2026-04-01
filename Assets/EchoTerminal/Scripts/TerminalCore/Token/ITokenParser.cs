using System;

public interface ITokenParser
{
	Type Type { get; }
	TokenState ParseTokenState(string raw);
	object ParseValue(string raw);
}