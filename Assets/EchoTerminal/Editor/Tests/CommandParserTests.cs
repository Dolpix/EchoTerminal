using System.Collections.Generic;
using System.Linq;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using EchoTerminal.Scripts.TerminalCore.Command;
using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests
{
[TestFixture]
public class CommandParserTests
{
	private CommandParser _parser;

	[SetUp]
	public void SetUp()
	{
		var registry = new CommandRegistry();
		registry.RegisterType(typeof(TestCommands));
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(registry.GetCommandNames()));
		var tokenizer = new Tokenizer(ParserRegistry.CreateAllParsers());
		_parser = new(registry, tokenizer);
	}

	[TestCase("XyzUnknown", TokenState.Failed)]
	[TestCase("XyzUnknown 42", TokenState.Failed)]
	[TestCase("zzz 1 2 3", TokenState.Failed)]
	[TestCase("123cmd", TokenState.Failed)]
	[TestCase("T 1 2 3", TokenState.Failed)]
	[TestCase("T", TokenState.Partial)]
	[TestCase("TstI", TokenState.Partial)]
	[TestCase("Tst", TokenState.Partial)]
	[TestCase("TstN", TokenState.Partial)]
	public void Parse_UnknownCommand(string input, TokenState expectedCmdState)
	{
		CommandParseResult result = _parser.Parse(input);
		Assert.IsNull(result.Entries, "Entries must be null for unknown command");
		Assert.IsNull(result.Entry, "Entry must be null for unknown command");
		Assert.IsFalse(result.IsMatch, "IsMatch must be false for unknown command");
		Assert.AreEqual(expectedCmdState, result.CommandToken.State);
	}

	[TestCase("TstInt")]
	[TestCase("TstInt ")]
	[TestCase("TstInt abc")]
	[TestCase("TstInt 3.14")]
	[TestCase("TstNoArgs extra")]
	[TestCase("TstInt 1 2")]
	[TestCase("TstBool true false")]
	[TestCase("TstFloats 1 2 3 4")]
	[TestCase("TstBool maybe")]
	[TestCase("TstIntList [abc]")]
	[TestCase("TstIntList [1,2,abc]")]
	[TestCase("TstBound ><")]
	[TestCase("TstVec (1, 2, ")]
	[TestCase("TstBool tr")]
	[TestCase("TstBool tru")]
	[TestCase("TstIntList [1,2,")]
	[TestCase("TstBound >TstNoArgs")]
	[TestCase("TstBound >TstInt 4")]
	public void Parse_KnownCommand_NoMatch(string input)
	{
		CommandParseResult result = _parser.Parse(input);
		Assert.IsNotNull(result.Entries, "Entries must be set for a recognised command");
		Assert.IsNull(result.Entry, "Entry must be null when no overload matched");
		Assert.IsFalse(result.IsMatch, "IsMatch must be false when args do not satisfy any overload");
		Assert.AreEqual(
			TokenState.Completed,
			result.CommandToken.State,
			"Command token must be Completed for a known command"
		);
	}

	[TestCase("TstNoArgs", 0)]
	[TestCase("TstInt 42", 1)]
	[TestCase("TstInt -7", 1)]
	[TestCase("TstInt 0", 1)]
	[TestCase("TstBool true", 1)]
	[TestCase("TstBool false", 1)]
	[TestCase("TstStr \"hello world\"", 1)]
	[TestCase("TstVec (1, 2, 3)", 1)]
	[TestCase("TstFloats 1.0 2.0 3.0", 3)]
	[TestCase("TstOverload (1, 2, 3)", 1)]
	[TestCase("TstOverload 1.0 2.0 3.0", 3)]
	[TestCase("TstIntList []", 1)]
	[TestCase("TstIntList [1,2,3]", 1)]
	[TestCase("TstIntList [-1,0,99]", 1)]
	[TestCase("TstBound >TstNoArgs<", 1)]
	[TestCase("TstBound >TstInt 42<", 1)]
	[TestCase("TstBound >TstBool true<", 1)]
	public void Parse_KnownCommand_FullMatch(string input, int expectedArgCount)
	{
		CommandParseResult result = _parser.Parse(input);
		Assert.IsTrue(result.IsMatch, "IsMatch must be true for a valid command+args");
		Assert.IsNotNull(result.Entry, "Entry must be set on a successful match");
		Assert.IsNotNull(result.Entries, "Entries must be set for a recognised command");
		Assert.AreEqual(TokenState.Completed, result.CommandToken.State, "Command token must be Completed");
		Assert.AreEqual(expectedArgCount, result.ArgTokens.Count, "Arg token count must match parameter count");
		Assert.IsTrue(result.ArgTokens.All(t => t.State == TokenState.Completed),
			"All arg tokens must be Completed on a match");
	}

	[TestCase("TstInt 42", 0, TokenState.Completed)]
	[TestCase("TstBool true", 0, TokenState.Completed)]
	[TestCase("TstVec (1, 2, 3)", 0, TokenState.Completed)]
	[TestCase("TstFloats 1.0 2.0 3.0", 0, TokenState.Completed)]
	[TestCase("TstFloats 1.0 2.0 3.0", 1, TokenState.Completed)]
	[TestCase("TstFloats 1.0 2.0 3.0", 2, TokenState.Completed)]
	[TestCase("TstBool tr", 0, TokenState.Partial)]
	[TestCase("TstBool tru", 0, TokenState.Partial)]
	[TestCase("TstVec (1, 2, ", 0, TokenState.Partial)]
	[TestCase("TstInt abc", 0, TokenState.Failed)]
	[TestCase("TstInt 3.14", 0, TokenState.Failed)]
	[TestCase("TstVec (1, 2, )", 0, TokenState.Failed)]
	[TestCase("TstNoArgs extra", 0, TokenState.Failed)]
	[TestCase("TstInt 1 2", 0, TokenState.Completed)]
	[TestCase("TstInt 1 2", 1, TokenState.Failed)]
	public void Parse_ArgToken_State_ByIndex(string input, int argIndex, TokenState expected)
	{
		CommandParseResult result = _parser.Parse(input);
		Assert.IsNotNull(result.ArgTokens, $"ArgTokens must not be null for '{input}'");
		Assert.AreEqual(expected, result.ArgTokens[argIndex].State);
	}

	[Test]
	public void GetError_UnknownCommand_ContainsCommandName()
	{
		StringAssert.Contains("XyzUnknown", _parser.Parse("XyzUnknown 42").GetError());
	}

	[Test]
	public void GetError_KnownCommandBadArgs_ContainsCommandName()
	{
		StringAssert.Contains("TstInt", _parser.Parse("TstInt abc").GetError());
	}

	private static class TestCommands
	{
		[TerminalCommand("TstNoArgs")]
		private static void NoArgs()
		{
		}

		[TerminalCommand("TstInt")]
		private static void SetInt(int value)
		{
		}

		[TerminalCommand("TstBool")]
		private static void SetBool(bool value)
		{
		}

		[TerminalCommand("TstStr")]
		private static void SetStr(string value)
		{
		}

		[TerminalCommand("TstVec")]
		private static void SetVec(Vector3 value)
		{
		}

		[TerminalCommand("TstFloats")]
		private static void SetFloats(float x, float y, float z)
		{
		}

		[TerminalCommand("TstOverload")]
		private static void OverloadVec(Vector3 pos)
		{
		}

		[TerminalCommand("TstOverload")]
		private static void OverloadFloats(float x, float y, float z)
		{
		}

		[TerminalCommand("TstIntList")]
		private static void SetIntList(List<int> values)
		{
		}

		[TerminalCommand("TstBound")]
		private static void SetBound(BoundCommand command)
		{
		}
	}
}
}