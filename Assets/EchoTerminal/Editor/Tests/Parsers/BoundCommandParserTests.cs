using System.Linq;
using EchoTerminal.TerminalCore;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class BoundCommandParserTests
{
	[SetUp]
	public void SetUp()
	{
		var registry = new CommandRegistry();
		registry.Scan();
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(registry.GetCommandNames()));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(new[] { "@Player", "@Enemy1" }));
		var parsers = ParserRegistry.CreateAllParsers();
		var tokenizer = new Tokenizer(parsers);
		_ = new CommandParser(registry, tokenizer);
		_parser = parsers.OfType<BoundCommandParser>().First();
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

	private BoundCommandParser _parser;

	[Test]
	public void Type_IsBoundCommand()
	{
		Assert.AreEqual(typeof(BoundCommand), _parser.Type);
	}

	[TestCase(">Teleport (0, 0, 0)<", TokenState.Completed)]      // valid command + vec3
	[TestCase(">Spawn 42<", TokenState.Completed)]                // valid command + int
	[TestCase(">Spawn 3.14<", TokenState.Completed)]              // valid command + float
	[TestCase(">Spawn hello<", TokenState.Completed)]             // valid command + string
	[TestCase(">Kill<", TokenState.Completed)]                    // valid command, no args required
	[TestCase(">Ki", TokenState.Partial)]                         // command name prefix, open
	[TestCase(">Teleport (0, 0", TokenState.Partial)]             // vec3 in-progress, open
	[TestCase(">", TokenState.Partial)]                           // just opened
	[TestCase(">Teleport", TokenState.Partial)]                   // command name done, waiting for arg
	[TestCase(">Teleport (0, 0,", TokenState.Partial)]            // vec3 mid-component, open
	[TestCase(">Spawn 4", TokenState.Partial)]                    // arg in-progress, open
	[TestCase(">Teleport<", TokenState.Failed)]                   // closed, but Teleport requires a vec3
	[TestCase(">Tele<", TokenState.Failed)]                       // closed, partial command name
	[TestCase(">Teleport (0, ad", TokenState.Failed)]             // malformed vec3 component, open
	[TestCase(">Teleport (0, 0, 0) @Player<", TokenState.Failed)] // too many args
	[TestCase("", TokenState.Failed)]                             // no opening >
	[TestCase("Teleport", TokenState.Failed)]                     // no opening >
	[TestCase("42", TokenState.Failed)]                           // no opening >
	[TestCase("(0, 0, 0)", TokenState.Failed)]                    // no opening >
	[TestCase("><", TokenState.Failed)]                           // empty inner
	[TestCase(">   <", TokenState.Failed)]                        // whitespace inner
	[TestCase(">Teleport (0, 0, )<", TokenState.Failed)]          // malformed vec3, closed
	public void ParseTokenState_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase(">Teleport (0, 0, 0)<", "Teleport (0, 0, 0)", 2)]
	[TestCase(">Spawn 42<", "Spawn 42", 2)]
	[TestCase(">Kill<", "Kill", 1)]
	[TestCase(">Teleport @Player<", "Teleport @Player", 2)]
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
		Assert.AreEqual(typeof(CommandName), result.Tokens[0].Type);
	}
}
}