using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class StringParserTests
{
	private StringParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

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
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}
}