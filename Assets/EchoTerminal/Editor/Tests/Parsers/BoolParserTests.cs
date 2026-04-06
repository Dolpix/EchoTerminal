using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class BoolParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private BoolParser _parser;

	[Test]
	public void Type_IsBool()
	{
		Assert.AreEqual(typeof(bool), _parser.Type);
	}

	[TestCase("true", TokenState.Resolved)]
	[TestCase("false", TokenState.Resolved)]
	[TestCase("True", TokenState.Unresolved)]
	[TestCase("False", TokenState.Unresolved)]
	[TestCase("TRUE", TokenState.Unresolved)]
	[TestCase("tru", TokenState.Unresolved)]
	[TestCase("fals", TokenState.Unresolved)]
	[TestCase("yes", TokenState.Unresolved)]
	[TestCase("1", TokenState.Unresolved)]
	[TestCase("0", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void BoolParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("true", true)]
	[TestCase("false", false)]
	public void BoolParseValue_ReturnsExpectedBool(string raw, bool expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}