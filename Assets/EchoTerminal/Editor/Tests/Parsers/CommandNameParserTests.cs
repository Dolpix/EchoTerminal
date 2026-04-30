using System;
using EchoTerminal.Scripts.TerminalCore.Token;
using NUnit.Framework;
using Parser = EchoTerminal.Scripts.TerminalCore.Token.TokenParser.CommandNameParser;
using CmdName = EchoTerminal.Scripts.TerminalCore.Token.TokenParser.CommandName;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class CommandNameParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new(new[] { "Teleport", "Spawn", "Kill" });
	}

	private Parser _parser;

	[Test]
	public void Type_IsCommandName()
	{
		Assert.AreEqual(typeof(CmdName), _parser.Type);
	}

	[TestCase("Teleport", TokenState.Completed)]
	[TestCase("Spawn", TokenState.Completed)]
	[TestCase("Kill", TokenState.Completed)]
	[TestCase("Tele", TokenState.Partial)]
	[TestCase("Unknown", TokenState.Failed)]
	[TestCase("teleport", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void CommandNameParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void CommandNameParse_EmptyRegistry_ReturnsFailed()
	{
		var empty = new Parser(Array.Empty<string>());
		Assert.AreEqual(TokenState.Failed, empty.ParseTokenState("Teleport"));
	}

	[TestCase("Teleport")]
	[TestCase("Spawn")]
	[TestCase("Kill")]
	public void CommandNameParseValue_ReturnsCommandNameWithMatchingValue(string raw)
	{
		var result = (CmdName)_parser.ParseValue(raw);
		Assert.AreEqual(raw, result.Value);
	}
}
}