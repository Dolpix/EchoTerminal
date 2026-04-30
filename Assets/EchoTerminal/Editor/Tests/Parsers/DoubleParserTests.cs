using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class DoubleParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private DoubleParser _parser;

	[Test]
	public void Type_IsDouble()
	{
		Assert.AreEqual(typeof(double), _parser.Type);
	}

	[TestCase("3.14", TokenState.Completed)]
	[TestCase("42", TokenState.Completed)]
	[TestCase("-1.5", TokenState.Completed)]
	[TestCase("0.0", TokenState.Completed)]
	[TestCase("0", TokenState.Completed)]
	[TestCase("1.7976931348623157E+308", TokenState.Completed)]
	[TestCase("1.2.3", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("@Player", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void DoubleParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("3.14", 3.14)]
	[TestCase("42", 42.0)]
	[TestCase("-1.5", -1.5)]
	[TestCase("0.0", 0.0)]
	[TestCase("0", 0.0)]
	public void DoubleParseValue_ReturnsExpectedDouble(string raw, double expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}