using System;
using System.Collections.Generic;
using EchoTerminal;
using EchoTerminal.TerminalCore;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TokenizerTests
{
	private Tokenizer _tokenizer;

	[SetUp]
	public void SetUp()
	{
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(new LiteralTargetProvider("@Player", "@Enemy1", "@Enemy2")));
		_tokenizer = new(ParserRegistry.CreateAllParsers());
	}

	[TestCase("Teleport @Player (10, 0, 5)", 3)]
	[TestCase("Kill @Enemy1", 2)]
	[TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 4)]
	[TestCase("", 0)]
	[TestCase("   ", 0)]
	[TestCase("  Kill @Enemy1", 2)]
	public void Tokenize_ProducesCorrectTokenCount(string input, int expected)
	{
		Assert.AreEqual(expected, _tokenizer.Tokenize(input).Count);
	}

	[TestCase("Teleport @Player (10, 0, 5)", 0, "Teleport")]
	[TestCase("Teleport @Player (10, 0, 5)", 1, "@Player")]
	[TestCase("Teleport @Player (10, 0, 5)", 2, "(10, 0, 5)")]
	[TestCase("Spawn Goblin (1, 2, 3)", 2, "(1, 2, 3)")]
	[TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 2, "\"Hello World!\"")]
	[TestCase("  Kill @Enemy1", 0, "Kill")]
	[TestCase("        Spawn Goblin", 0, "Spawn")]
	public void Tokenize_RawValues_AreCorrect(string input, int index, string expected)
	{
		Assert.AreEqual(expected, _tokenizer.Tokenize(input)[index].Raw);
	}

	[TestCase("Teleport", 0, typeof(CommandName))]
	[TestCase("Spawn Goblin (1, 2, 3)", 0, typeof(CommandName))]
	[TestCase("Teleport @Player (10, 0, 5)", 1, typeof(Target))]
	[TestCase("Spawn Goblin", 1, typeof(string))]
	[TestCase("Spawn Goblin (1, 2, 3)", 2, typeof(Vector3))]
	[TestCase("Spawn 42", 1, typeof(int))]
	[TestCase("Spawn -10", 1, typeof(int))]
	[TestCase("Spawn 3.14", 1, typeof(float))]
	[TestCase("Spawn -1.5", 1, typeof(float))]
	[TestCase("Spawn red 3", 1, typeof(Color))]
	[TestCase("Spawn #FF0000 3", 1, typeof(Color))]
	[TestCase("Spawn #00FF00FF 3", 1, typeof(Color))]
	public void Tokenize_ResolvesCorrectTypes(string input, int index, Type expected)
	{
		Assert.AreEqual(expected, _tokenizer.Tokenize(input)[index].ExpectedType);
	}

	[TestCase("Spawn (1,0,0)", 1, typeof(Color))]
	[TestCase("Spawn (0,1,0,0.5)", 1, typeof(Color))]
	public void Tokenize_ColorTuple_WithExpectedType_ResolvesAsColor(string input, int index, Type expected)
	{
		List<Token> tokens = _tokenizer.Tokenize(input, new() { null, typeof(Color) });
		Assert.AreEqual(expected, tokens[index].ExpectedType);
		Assert.AreEqual(TokenState.Completed, tokens[index].State);
	}

	[TestCase("42", typeof(int))]
	[TestCase("3.14", typeof(float))]
	public void Tokenize_AmbiguousNumber_ResolvesToMoreSpecificType(string input, Type expected)
	{
		List<Token> tokens = _tokenizer.Tokenize(input);
		Assert.AreEqual(1, tokens.Count);
		Assert.AreEqual(expected, tokens[0].ExpectedType);
	}

	[TestCase("42", 1, 0, TokenState.Completed)]        // valid int arg
	[TestCase("3.14", 1, 0, TokenState.Completed)]      // float — wrong for int ctx, still Completed (no hint)
	[TestCase("abc", 1, 0, TokenState.Completed)]       // wrong type → resolves as string
	[TestCase("tr", 1, 0, TokenState.Completed)]        // partial bool input → resolves as string (no hint)
	[TestCase("extra", 1, 0, TokenState.Completed)]     // too many, 1 extra → string
	[TestCase("1 2", 2, 0, TokenState.Completed)]       // too many, first of two extras
	[TestCase("1 2", 2, 1, TokenState.Completed)]       // too many, second of two extras
	[TestCase("(1, 2, 3)", 1, 0, TokenState.Completed)] // complete vec3
	[TestCase("(1, 2, ", 1, 0, TokenState.Partial)]     // vec3 mid-type
	[TestCase("(1, 2, )", 1, 0, TokenState.Failed)]     // malformed vec3
	public void Tokenize_RawArgString_HasExpectedState(
		string argInput,
		int expectedCount,
		int index,
		TokenState expected)
	{
		List<Token> tokens = _tokenizer.Tokenize(argInput);
		Assert.AreEqual(expectedCount, tokens.Count, $"Token count for '{argInput}'");
		Assert.AreEqual(expected, tokens[index].State, $"Token[{index}].State for '{argInput}'");
	}

	[TestCase("Teleport @Player (10, 0, 5)", 0, TokenState.Completed)]
	[TestCase("Teleport @Player (10, 0, 5)", 1, TokenState.Completed)]
	[TestCase("Teleport @Player (10, 0, 5)", 2, TokenState.Completed)]
	[TestCase("Teleport", 0, TokenState.Completed)]
	[TestCase("Kill @Enemy1 \"Hello\"", 2, TokenState.Completed)]
	[TestCase("Teleport @Player (10, 0, ", 0, TokenState.Completed)]
	[TestCase("Teleport @Player (10, 0, ", 1, TokenState.Completed)]
	[TestCase("Teleport @Player (10, 0, ", 2, TokenState.Partial)]
	[TestCase("Kill @Enemy1 \"Hello", 2, TokenState.Partial)]
	[TestCase("Teleport @Player (10, 0, )", 2, TokenState.Failed)]
	public void Tokenize_TokenState_ByIndex(string input, int index, TokenState expected)
	{
		Assert.AreEqual(expected, _tokenizer.Tokenize(input)[index].State);
	}
}