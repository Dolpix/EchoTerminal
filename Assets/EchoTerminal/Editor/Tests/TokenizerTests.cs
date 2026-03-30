using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using EchoTerminal.TerminalCore;

[TestFixture]
public class TokenizerTests
{
	private List<ITokenParser> _parsers;

	[SetUp]
	public void SetUp()
	{
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(new[] { "@Player", "@Enemy1", "@Enemy2" }));
		_parsers = ParserRegistry.CreateAll();
	}

	[TestCase("Teleport @Player (10, 0, 5)", 3)]
	[TestCase("Kill @Enemy1", 2)]
	[TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 4)]
	[TestCase("", 0)]
	[TestCase("   ", 0)]
	[TestCase("  Kill @Enemy1", 2)]
	public void Tokenize_ProducesCorrectTokenCount(string input, int expected)
	{
		Assert.AreEqual(expected, Tokenizer.Tokenize(input, _parsers).Count);
	}

	[Test]
	public void LeadingSpaces_DoNotAffectFirstTokenRaw()
	{
		var tokens = Tokenizer.Tokenize("  Kill @Enemy1", _parsers);
		Assert.AreEqual("Kill", tokens[0].Raw);
	}

	[TestCase("Spawn Goblin (1, 2, 3)", 2, "(1, 2, 3)")]
	[TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 2, "\"Hello World!\"")]
	public void PendingParser_SpansInternalSpaces_IntoSingleToken(string input, int index, string expectedRaw)
	{
		Assert.AreEqual(expectedRaw, Tokenizer.Tokenize(input, _parsers)[index].Raw);
	}

	[Test]
	public void Tokenize_RawValues_AreCorrect()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual("Teleport", tokens[0].Raw);
		Assert.AreEqual("@Player", tokens[1].Raw);
		Assert.AreEqual("(10, 0, 5)", tokens[2].Raw);
	}

	[Test]
	public void Tokenize_ResolvesCorrectTypes()
	{
		var tokens = Tokenizer.Tokenize("Spawn Goblin (1, 2, 3)", _parsers);
		Assert.AreEqual(typeof(CommandName), tokens[0].Type);
		Assert.AreEqual(typeof(string), tokens[1].Type);
		Assert.AreEqual(typeof(Vector3), tokens[2].Type);
	}

	[Test]
	public void Tokenize_SpaceTerminatedTokens_AreResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual(TokenState.Resolved, tokens[0].State);
		Assert.AreEqual(TokenState.Resolved, tokens[1].State);
	}

	[TestCase("Teleport @Player (10, 0, 5)", TokenState.Resolved)]
	[TestCase("Teleport @Player (10, 0, ", TokenState.Pending)]
	[TestCase("Teleport @Player (10, 0, )", TokenState.Invalid)]
	public void Tokenize_LastToken_HasExpectedState(string input, TokenState expected)
	{
		var tokens = Tokenizer.Tokenize(input, _parsers);
		Assert.AreEqual(expected, tokens[^1].State);
	}

	[Test]
	public void Tokenize_SingleKnownCommand_IsResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport", _parsers);
		Assert.AreEqual(1, tokens.Count);
		Assert.AreEqual(TokenState.Resolved, tokens[0].State);
	}

	[TestCase("42",   typeof(int))]
	[TestCase("3.14", typeof(float))]
	public void Tokenize_AmbiguousNumber_ResolvesToMoreSpecificType(string input, System.Type expected)
	{
		var tokens = Tokenizer.Tokenize(input, _parsers);
		Assert.AreEqual(expected, tokens[0].Type);
	}
}