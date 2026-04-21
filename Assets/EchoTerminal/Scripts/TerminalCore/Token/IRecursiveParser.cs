using System;
using System.Collections.Generic;

public interface IRecursiveParser
{
	List<Token> GetSubTokens(string raw, Type expectedType);
}