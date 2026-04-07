using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class LongParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private LongParser _parser;

	[Test]
	public void Type_IsLong()
	{
		Assert.AreEqual(typeof(long), _parser.Type);
	}

	[TestCase("42", TokenState.Resolved)]
	[TestCase("0", TokenState.Resolved)]
	[TestCase("-10", TokenState.Resolved)]
	[TestCase("9999999999999", TokenState.Resolved)]
	[TestCase("-9999999999999", TokenState.Resolved)]
	[TestCase("3.14", TokenState.Unresolved)]
	[TestCase("1.0", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void LongParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("42", 42L)]
	[TestCase("0", 0L)]
	[TestCase("-10", -10L)]
	[TestCase("9999999999999", 9999999999999L)]
	[TestCase("-9999999999999", -9999999999999L)]
	public void LongParseValue_ReturnsExpectedLong(string raw, long expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}