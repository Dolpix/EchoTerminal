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

	[TestCase("42", TokenState.Completed)]
	[TestCase("0", TokenState.Completed)]
	[TestCase("-10", TokenState.Completed)]
	[TestCase("9999999999999", TokenState.Completed)]
	[TestCase("-9999999999999", TokenState.Completed)]
	[TestCase("3.14", TokenState.Failed)]
	[TestCase("1.0", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("@Player", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
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