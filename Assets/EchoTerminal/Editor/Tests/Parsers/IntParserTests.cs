using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class IntParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private IntParser _parser;

	[Test]
	public void Type_IsInt()
	{
		Assert.AreEqual(typeof(int), _parser.Type);
	}

	[TestCase("42", TokenState.Resolved)]
	[TestCase("0", TokenState.Resolved)]
	[TestCase("-10", TokenState.Resolved)]
	[TestCase("999999", TokenState.Resolved)]
	[TestCase("3.14", TokenState.Unresolved)]
	[TestCase("1.0", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("42", 42)]
	[TestCase("0", 0)]
	[TestCase("-10", -10)]
	[TestCase("999999", 999999)]
	public void IntParseValue_ReturnsExpectedInt(string raw, int expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}