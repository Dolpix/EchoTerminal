using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TokenizerTests
{
	private List<ITokenParser> _parsers;

	[SetUp]
	public void SetUp()
	{
		_parsers = new()
		{
			new IntParser(),
			new FloatParser(),
			new TargetParser(new[] { "@Player", "@Enemy1", "@Enemy2" }),
			new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }),
			new QuotedStringParser(),
			new Vec3Parser(),
			new StringParser()
		};
	}

	[Test]
	public void SimpleCommand_ProducesCorrectTokenCount()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual(3, tokens.Count);
	}

	[Test]
	public void TwoWordCommand_ProducesCorrectTokenCount()
	{
		var tokens = Tokenizer.Tokenize("Kill @Enemy1", _parsers);
		Assert.AreEqual(2, tokens.Count);
	}

	[Test]
	public void CommandWithQuotedString_ProducesCorrectTokenCount()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player \"Hello World!\" (0, 0, 0)", _parsers);
		Assert.AreEqual(4, tokens.Count);
	}

	[Test]
	public void SimpleCommand_TokenRawValues_AreCorrect()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual("Teleport", tokens[0].Raw);
		Assert.AreEqual("@Player", tokens[1].Raw);
		Assert.AreEqual("(10, 0, 5)", tokens[2].Raw);
	}

	[Test]
	public void Vec3_SpansInternalSpaces_IntoSingleToken()
	{
		var tokens = Tokenizer.Tokenize("Spawn Goblin (1, 2, 3)", _parsers);
		Assert.AreEqual("(1, 2, 3)", tokens[2].Raw);
	}

	[Test]
	public void CommandNameTypeTest()
	{
		var tokens = Tokenizer.Tokenize("Spawn Goblin (1, 2, 3)", _parsers);
		Assert.AreEqual(typeof(CommandName), tokens[0].Type);
		Assert.AreEqual(typeof(string), tokens[1].Type);
		Assert.AreEqual(typeof(Vector3), tokens[2].Type);
	}


	[Test]
	public void QuotedString_SpansInternalSpaces_IntoSingleToken()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player \"Hello World!\" (0, 0, 0)", _parsers);
		Assert.AreEqual("\"Hello World!\"", tokens[2].Raw);
	}

	[Test]
	public void AllTokensExceptLast_IsResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual(TokenState.Resolved, tokens[0].State);
		Assert.AreEqual(TokenState.Resolved, tokens[1].State);
	}

	[Test]
	public void LastToken_IsResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, 5)", _parsers);
		Assert.AreEqual(TokenState.Resolved, tokens[^1].State);
	}

	[Test]
	public void LastToken_IsUnResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, ", _parsers);
		Assert.AreNotEqual(TokenState.Resolved, tokens[^1].State);
		Assert.AreEqual(TokenState.Pending, tokens[^1].State);
	}

	[Test]
	public void LastToken_IsInvalid()
	{
		var tokens = Tokenizer.Tokenize("Teleport @Player (10, 0, )", _parsers);
		Assert.AreNotEqual(TokenState.Resolved, tokens[^1].State);
		Assert.AreEqual(TokenState.Invalid, tokens[^1].State);
	}

	[Test]
	public void SingleToken_IsResolved()
	{
		var tokens = Tokenizer.Tokenize("Teleport", _parsers);
		Assert.AreEqual(1, tokens.Count);
		Assert.AreEqual(TokenState.Resolved, tokens[0].State);
	}

	[Test]
	public void EmptyString_ReturnsNoTokens()
	{
		var tokens = Tokenizer.Tokenize("", _parsers);
		Assert.AreEqual(0, tokens.Count);
	}

	[Test]
	public void OnlySpaces_ReturnsNoTokens()
	{
		var tokens = Tokenizer.Tokenize("   ", _parsers);
		Assert.AreEqual(0, tokens.Count);
	}

	[Test]
	public void LeadingSpaces_AreIgnored()
	{
		var tokens = Tokenizer.Tokenize("  Kill @Enemy1", _parsers);
		Assert.AreEqual(2, tokens.Count);
		Assert.AreEqual("Kill", tokens[0].Raw);
	}
}