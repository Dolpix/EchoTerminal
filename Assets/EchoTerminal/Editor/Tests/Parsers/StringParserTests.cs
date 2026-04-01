using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class StringParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private StringParser _parser;

	[Test]
	public void Type_IsString()
	{
		Assert.AreEqual(typeof(string), _parser.Type);
	}

	[TestCase("Goblin", TokenState.Resolved)]
	[TestCase("Gob", TokenState.Resolved)]
	[TestCase("\"Hello World\"", TokenState.Resolved)]
	[TestCase("\"Hello\"", TokenState.Resolved)]
	[TestCase("\"\"", TokenState.Resolved)]
	[TestCase("\"Hello", TokenState.Pending)]
	[TestCase("\"", TokenState.Pending)]
	[TestCase("123abc", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void StringParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("Goblin", "Goblin")]
	[TestCase("Gob", "Gob")]
	[TestCase("\"Hello World\"", "Hello World")]
	[TestCase("\"Hello\"", "Hello")]
	[TestCase("\"\"", "")]
	public void StringParseValue_ReturnsExpectedString(string raw, string expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}