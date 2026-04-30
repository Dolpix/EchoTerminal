using EchoTerminal.Scripts.TerminalCore.Targets;
using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class TargetParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new(new LiteralTargetProvider("@Player", "@Enemy1", "@Enemy2"));
	}

	private TargetParser _parser;

	[Test]
	public void Type_IsTarget()
	{
		Assert.AreEqual(typeof(Target), _parser.Type);
	}

	[TestCase("@Player", TokenState.Completed)]
	[TestCase("@Enemy1", TokenState.Completed)]
	[TestCase("@Enemy2", TokenState.Completed)]
	[TestCase("@P", TokenState.Partial)]
	[TestCase("@Pla", TokenState.Partial)]
	[TestCase("@Play", TokenState.Partial)]
	[TestCase("@Player3", TokenState.Failed)]
	[TestCase("@Unknown", TokenState.Failed)]
	[TestCase("@XYZ", TokenState.Failed)]
	[TestCase("@Z", TokenState.Failed)]
	[TestCase("Player", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	public void TargetParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("@Player")]
	[TestCase("@Enemy1")]
	[TestCase("@Enemy2")]
	public void TargetParseValue_ReturnsTargetWithMatchingValue(string raw)
	{
		var result = (Target)_parser.ParseValue(raw);
		Assert.AreEqual(raw, result.Value);
	}
}
}