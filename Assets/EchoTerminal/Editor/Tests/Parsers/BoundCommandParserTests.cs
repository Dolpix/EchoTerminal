using System.Collections.Generic;
using System.Linq;
using EchoTerminal.TerminalCore;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class BoundCommandParserTests
{
	private BoundCommandParser _parser;

	[SetUp]
	public void SetUp()
	{
		var registry = new CommandRegistry();
		registry.RegisterType(typeof(TestCommands));
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(registry.GetCommandNames()));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(new LiteralTargetProvider("@Player", "@Enemy1")));
		List<ITokenParser> parsers = ParserRegistry.CreateAllParsers();
		var tokenizer = new Tokenizer(parsers);
		_ = new CommandParser(registry, tokenizer);
		_parser = parsers.OfType<BoundCommandParser>().First();
	}

	[Test]
	public void Type_IsBoundCommand()
	{
		Assert.AreEqual(typeof(BoundCommand), _parser.Type);
	}

	[TestCase(">Teleport (0, 0, 0)<", TokenState.Completed)]
	[TestCase(">Spawn 42<", TokenState.Completed)]
	[TestCase(">Spawn 3.14<", TokenState.Completed)]
	[TestCase(">Spawn hello<", TokenState.Completed)]
	[TestCase(">Kill<", TokenState.Completed)]
	[TestCase(">Ki", TokenState.Partial)]
	[TestCase(">Teleport (0, 0", TokenState.Partial)]
	[TestCase(">", TokenState.Partial)]
	[TestCase(">Teleport", TokenState.Partial)]
	[TestCase(">Teleport (0, 0,", TokenState.Partial)]
	[TestCase(">Spawn 4", TokenState.Partial)]
	[TestCase(">Teleport<", TokenState.Failed)]
	[TestCase(">Tele<", TokenState.Failed)]
	[TestCase(">Teleport (0, ad", TokenState.Failed)]
	[TestCase(">Teleport (0, 0, 0) @Player<", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	[TestCase("Teleport", TokenState.Failed)]
	[TestCase("42", TokenState.Failed)]
	[TestCase("(0, 0, 0)", TokenState.Failed)]
	[TestCase("><", TokenState.Failed)]
	[TestCase(">   <", TokenState.Failed)]
	[TestCase(">Teleport (0, 0, )<", TokenState.Failed)]
	public void ParseTokenState_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase(">Teleport (0, 0, 0)<", "Teleport (0, 0, 0)", 2)]
	[TestCase(">Spawn 42<", "Spawn 42", 2)]
	[TestCase(">Kill<", "Kill", 1)]
	public void ParseValue_ReturnsPopulatedBoundCommand(string raw, string expectedRaw, int expectedTokenCount)
	{
		var result = (BoundCommand)_parser.ParseValue(raw);

		Assert.AreEqual(expectedRaw, result.Raw, "Raw string was not stripped correctly.");
		Assert.AreEqual(expectedRaw, result.ToString(), "ToString() does not match Raw.");
		Assert.AreEqual(expectedTokenCount, result.Tokens.Count, "Token count mismatch.");
		Assert.IsTrue(result.Tokens.All(t => t.State == TokenState.Completed),
			"Not all tokens are in a Completed state.");
	}

	[Test]
	public void ParseValue_FirstToken_IsCommandName()
	{
		var result = (BoundCommand)_parser.ParseValue(">Teleport (0, 0, 0)<");
		Assert.AreEqual(typeof(CommandName), result.Tokens[0].ExpectedType);
	}

	private static class TestCommands
	{
		[TerminalCommand("Kill")]
		private static void Kill()
		{
		}

		[TerminalCommand("Teleport")]
		private static void Teleport(Vector3 destination)
		{
		}

		[TerminalCommand("Teleport")]
		private static void TeleportTarget()
		{
		}

		[TerminalCommand("Spawn")]
		private static void SpawnInt(int id)
		{
		}

		[TerminalCommand("Spawn")]
		private static void SpawnFloat(float value)
		{
		}

		[TerminalCommand("Spawn")]
		private static void SpawnStr(string name)
		{
		}
	}
}
}