using System;

public struct Token
{
	public string Raw;
	public TokenState State;
	public Type Type;
	public Type ExpectedType;
	public string ErrorMessage;
}