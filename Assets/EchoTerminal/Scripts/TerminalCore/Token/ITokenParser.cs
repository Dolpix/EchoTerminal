using System;

public interface ITokenParser
{
	Type Type { get; }
	TokenState Parse(string raw);
}