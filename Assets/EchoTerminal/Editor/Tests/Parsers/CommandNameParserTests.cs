using System;
using NUnit.Framework;
using Parser = EchoTerminal.TerminalCore.CommandNameParser;
using CmdName = EchoTerminal.TerminalCore.CommandName;

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

	[TestCase("Teleport", TokenState.Resolved)]
	[TestCase("Spawn", TokenState.Resolved)]
	[TestCase("Kill", TokenState.Resolved)]
	[TestCase("Tele", TokenState.Unresolved)]
	[TestCase("teleport", TokenState.Unresolved)]
	[TestCase("Unknown", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void CommandNameParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void CommandNameParse_EmptyRegistry_ReturnsUnresolved()
	{
		var empty = new Parser(Array.Empty<string>());
		Assert.AreEqual(TokenState.Unresolved, empty.ParseTokenState("Teleport"));
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