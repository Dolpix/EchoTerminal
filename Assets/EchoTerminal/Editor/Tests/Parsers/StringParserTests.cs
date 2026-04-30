using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
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

	[TestCase("Goblin", TokenState.Completed)]
	[TestCase("Gob", TokenState.Completed)]
	[TestCase("\"Hello World\"", TokenState.Completed)]
	[TestCase("\"Hello\"", TokenState.Completed)]
	[TestCase("\"\"", TokenState.Completed)]
	[TestCase("\"Hello", TokenState.Partial)]
	[TestCase("\"", TokenState.Partial)]
	[TestCase("123abc", TokenState.Failed)]
	[TestCase("@Player", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
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