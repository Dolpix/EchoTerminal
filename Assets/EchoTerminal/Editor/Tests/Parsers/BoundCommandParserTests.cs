using System.Linq;
using EchoTerminal.TerminalCore;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class BoundCommandParserTests
{
	[SetUp]
	public void SetUp()
	{
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(new[] { "Teleport", "Spawn", "Kill" }));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(new[] { "@Player", "@Enemy1" }));
		_parser = ParserRegistry.CreateAllParsers().OfType<BoundCommandParser>().First();
	}

	private BoundCommandParser _parser;

	[Test]
	public void Type_IsBoundCommand()
	{
		Assert.AreEqual(typeof(BoundCommand), _parser.Type);
	}

	[TestCase(">Teleport (0, 0, 0)<", TokenState.Completed)]
	[TestCase(">Kill @Enemy1<", TokenState.Completed)]
	[TestCase(">Spawn 42<", TokenState.Completed)]
	[TestCase(">Spawn 3.14<", TokenState.Completed)]
	[TestCase(">Spawn hello<", TokenState.Completed)]
	[TestCase(">Kill<", TokenState.Completed)]
	[TestCase(">Teleport (0, 0, 0) @Player<", TokenState.Completed)]
	public void ParseTokenState_ValidCommand_ReturnsCompleted(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase(">")]
	[TestCase(">Teleport")]
	[TestCase(">Teleport (0, 0,")]
	[TestCase(">Spawn 4")]
	public void ParseTokenState_UnclosedDelimiter_ReturnsPartial(string raw)
	{
		Assert.AreEqual(TokenState.Partial, _parser.ParseTokenState(raw));
	}

	[TestCase("")]
	[TestCase("Teleport")]
	[TestCase("42")]
	[TestCase("(0, 0, 0)")]
	[TestCase("><")]
	[TestCase(">   <")]
	[TestCase(">Teleport (0, 0, )<")]
	public void ParseTokenState_NotRecognised_ReturnsFailed(string raw)
	{
		Assert.AreEqual(TokenState.Failed, _parser.ParseTokenState(raw));
	}

	[Test]
	public void ParseValue_Raw_StripsDelimiters()
	{
		var result = (BoundCommand)_parser.ParseValue(">Teleport (0, 0, 0)<");
		Assert.AreEqual("Teleport (0, 0, 0)", result.Raw);
	}

	[Test]
	public void ParseValue_ToString_MatchesRaw()
	{
		var result = (BoundCommand)_parser.ParseValue(">Spawn 42<");
		Assert.AreEqual("Spawn 42", result.ToString());
	}

	[Test]
	public void ParseValue_Tokens_ArePopulated()
	{
		var result = (BoundCommand)_parser.ParseValue(">Teleport (0, 0, 0)<");
		Assert.AreEqual(2, result.Tokens.Count);
	}

	[Test]
	public void ParseValue_AllTokens_AreCompleted()
	{
		var result = (BoundCommand)_parser.ParseValue(">Kill @Enemy1<");
		Assert.IsTrue(result.Tokens.All(t => t.State == TokenState.Completed));
	}

	[Test]
	public void ParseValue_FirstToken_IsCommandName()
	{
		var result = (BoundCommand)_parser.ParseValue(">Teleport (0, 0, 0)<");
		Assert.AreEqual(typeof(CommandName), result.Tokens[0].Type);
	}

	[Test]
	public void ParseValue_SingleWordCommand_HasOneToken()
	{
		var result = (BoundCommand)_parser.ParseValue(">Kill<");
		Assert.AreEqual(1, result.Tokens.Count);
		Assert.AreEqual("Kill", result.Tokens[0].Raw);
	}

	[Test]
	public void ParseValue_WithTarget_TokenCountCorrect()
	{
		var result = (BoundCommand)_parser.ParseValue(">Teleport @Player<");
		Assert.AreEqual(2, result.Tokens.Count);
	}
}
}