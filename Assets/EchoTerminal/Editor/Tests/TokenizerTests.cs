using System;
using EchoTerminal.TerminalCore;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TokenizerTests
{
    [SetUp]
    public void SetUp()
    {
        ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }));
        ParserRegistry.Register<TargetParser>(() => new TargetParser(new[] { "@Player", "@Enemy1", "@Enemy2" }));
        _tokenizer = new Tokenizer(ParserRegistry.CreateAllParsers());
    }

    private Tokenizer _tokenizer;

    [TestCase("Teleport @Player (10, 0, 5)", 3)]
    [TestCase("Kill @Enemy1", 2)]
    [TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 4)]
    [TestCase("", 0)]
    [TestCase("   ", 0)]
    [TestCase("  Kill @Enemy1", 2)]
    public void Tokenize_ProducesCorrectTokenCount(string input, int expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens.Count);
    }

    [TestCase("  Kill @Enemy1", 0, "Kill")]
    [TestCase("        Spawn Goblin", 0, "Spawn")]
    public void LeadingSpaces_DoNotAffectFirstTokenRaw(string input, int index, string expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens[index].Raw);
    }

    [TestCase("Spawn Goblin (1, 2, 3)", 2, "(1, 2, 3)")]
    [TestCase("Teleport @Player \"Hello World!\" (0, 0, 0)", 2, "\"Hello World!\"")]
    [TestCase("Kill @Enemy1 \"Open the door\"", 2, "\"Open the door\"")]
    public void PendingParser_SpansInternalSpaces_IntoSingleToken(string input, int index, string expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens[index].Raw);
    }

    [TestCase("Teleport @Player (10, 0, 5)", 0, "Teleport")]
    [TestCase("Teleport @Player (10, 0, 5)", 1, "@Player")]
    [TestCase("Teleport @Player (10, 0, 5)", 2, "(10, 0, 5)")]
    [TestCase("Spawn Goblin (1, 2, 3)", 0, "Spawn")]
    [TestCase("Spawn Goblin (1, 2, 3)", 1, "Goblin")]
    public void Tokenize_RawValues_AreCorrect(string input, int index, string expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens[index].Raw);
    }

    // CommandName
    [TestCase("Teleport", 0, typeof(CommandName))]
    [TestCase("Spawn", 0, typeof(CommandName))]
    [TestCase("Kill", 0, typeof(CommandName))]
    [TestCase("Spawn Goblin (1, 2, 3)", 0, typeof(CommandName))]
    // string — plain word
    [TestCase("Spawn Goblin (1, 2, 3)", 1, typeof(string))]
    [TestCase("Spawn Goblin", 1, typeof(string))]
    [TestCase("Kill Dragon", 1, typeof(string))]
    // string — target (@)
    [TestCase("Teleport @Player (10, 0, 5)", 1, typeof(string))]
    [TestCase("Teleport @Enemy1", 1, typeof(string))]
    [TestCase("Kill @Enemy2", 1, typeof(string))]
    // string — quoted
    [TestCase("Spawn \"Hello World\"", 1, typeof(string))]
    [TestCase("Kill \"test\"", 1, typeof(string))]
    // Vector3
    [TestCase("Spawn Goblin (1, 2, 3)", 2, typeof(Vector3))]
    [TestCase("Teleport @Player (10, 0, 5)", 2, typeof(Vector3))]
    [TestCase("Kill @Enemy1 (0, 0, 0)", 2, typeof(Vector3))]
    [TestCase("Kill @Enemy1 (-1, -2, -3)", 2, typeof(Vector3))]
    [TestCase("Kill @Enemy1 (1.5, 2.5, 3.5)", 2, typeof(Vector3))]
    // int
    [TestCase("Spawn 42", 1, typeof(int))]
    [TestCase("Spawn 0", 1, typeof(int))]
    [TestCase("Spawn -10", 1, typeof(int))]
    // float
    [TestCase("Spawn 3.14", 1, typeof(float))]
    [TestCase("Spawn -1.5", 1, typeof(float))]
    [TestCase("Spawn 0.0", 1, typeof(float))]
    public void Tokenize_ResolvesCorrectTypes(string input, int index, Type expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens[index].Type);
    }

    [TestCase("Teleport @Player (10, 0, 5)", 0, TokenState.Resolved)]
    [TestCase("Teleport @Player (10, 0, 5)", 1, TokenState.Resolved)]
    [TestCase("Teleport @Player (10, 0, 5)", 2, TokenState.Resolved)]
    [TestCase("Teleport", 0, TokenState.Resolved)]
    public void Tokenize_SpaceTerminatedTokens_AreResolved(string input, int index, TokenState expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(expected, tokens[index].State);
    }

    [TestCase("Teleport @Player (10, 0, 5)", TokenState.Resolved)]
    [TestCase("Teleport @Player (10, 0, ", TokenState.Pending)]
    [TestCase("Teleport @Player (10, 0, )", TokenState.Invalid)]
    [TestCase("Kill @Enemy1 \"Hello", TokenState.Pending)]
    [TestCase("Kill @Enemy1 \"Hello\"", TokenState.Resolved)]
    public void Tokenize_LastToken_HasExpectedState(string input, TokenState expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.Greater(tokens.Count, 0);
        Assert.AreEqual(expected, tokens[^1].State);
    }

    [TestCase("42", typeof(int))]
    [TestCase("3.14", typeof(float))]
    public void Tokenize_AmbiguousNumber_ResolvesToMoreSpecificType(string input, Type expected)
    {
        var tokens = _tokenizer.Tokenize(input);
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(expected, tokens[0].Type);
    }
}
