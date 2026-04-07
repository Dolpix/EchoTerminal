using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class CharParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private CharParser _parser;

	[Test]
	public void Type_IsChar()
	{
		Assert.AreEqual(typeof(char), _parser.Type);
	}

	[TestCase("'a'", TokenState.Resolved)]
	[TestCase("'z'", TokenState.Resolved)]
	[TestCase("'A'", TokenState.Resolved)]
	[TestCase("'0'", TokenState.Resolved)]
	[TestCase("'!'", TokenState.Resolved)]
	[TestCase("'", TokenState.Pending)]
	[TestCase("'a", TokenState.Pending)]
	[TestCase("''", TokenState.Invalid)]
	[TestCase("'ab'", TokenState.Invalid)]
	[TestCase("'abc'", TokenState.Invalid)]
	[TestCase("a", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void CharParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("'a'", 'a')]
	[TestCase("'z'", 'z')]
	[TestCase("'A'", 'A')]
	[TestCase("'0'", '0')]
	[TestCase("'!'", '!')]
	public void CharParseValue_ReturnsExpectedChar(string raw, char expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}