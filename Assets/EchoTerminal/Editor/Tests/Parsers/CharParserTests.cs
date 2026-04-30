using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
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

	[TestCase("'a'", TokenState.Completed)]
	[TestCase("'z'", TokenState.Completed)]
	[TestCase("'A'", TokenState.Completed)]
	[TestCase("'0'", TokenState.Completed)]
	[TestCase("'!'", TokenState.Completed)]
	[TestCase("'", TokenState.Partial)]
	[TestCase("'a", TokenState.Partial)]
	[TestCase("''", TokenState.Failed)]
	[TestCase("'ab'", TokenState.Failed)]
	[TestCase("'abc'", TokenState.Failed)]
	[TestCase("a", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
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