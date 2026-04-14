using System.Collections.Generic;
using EchoTerminal.TerminalCore;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests
{
[TestFixture]
public class CommandParserTests
{
	[SetUp]
	public void SetUp()
	{
		var registry = new CommandRegistry();
		registry.Scan();
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(registry.GetCommandNames()));
		var tokenizer = new Tokenizer(ParserRegistry.CreateAllParsers());
		_parser = new(registry, tokenizer);
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

	private CommandParser _parser;

	[TestCase("TstNoArgs")]
	[TestCase("TstInt 42")]
	[TestCase("TstInt -7")]
	[TestCase("TstInt 0")]
	[TestCase("TstBool true")]
	[TestCase("TstBool false")]
	[TestCase("TstStr \"hello world\"")]
	[TestCase("TstVec (1, 2, 3)")]
	[TestCase("TstFloats 1.0 2.0 3.0")]
	[TestCase("TstOverload (1, 2, 3)")]
	[TestCase("TstOverload 1.0 2.0 3.0")]
	[TestCase("TstIntList []")]
	[TestCase("TstIntList [1,2,3]")]
	[TestCase("TstIntList [-1,0,99]")]
	[TestCase("TstBound >TstNoArgs<")]
	[TestCase("TstBound >TstInt 42<")]
	[TestCase("TstBound >TstBool true<")]
	public void Parse_ValidInput_IsMatch(string input)
	{
		Assert.IsTrue(_parser.Parse(input).IsMatch);
	}

	[TestCase("XyzUnknown")]
	[TestCase("XyzUnknown 42")]
	[TestCase("zzz 1 2 3")]
	public void Parse_UnknownCommand_EntriesIsNull(string input)
	{
		Assert.IsNull(_parser.Parse(input).Entries);
	}

	[TestCase("TstInt")]
	[TestCase("TstInt abc")]
	[TestCase("TstInt 3.14")]
	[TestCase("TstNoArgs extra")]
	[TestCase("TstBool tr")]
	[TestCase("TstBool tru")]
	[TestCase("TstBool maybe")]
	[TestCase("TstIntList [abc]")]
	[TestCase("TstIntList [1,2,abc]")]
	[TestCase("TstBound ><")]
	public void Parse_KnownCommandBadArgs_EntriesNotNullNotMatch(string input)
	{
		var result = _parser.Parse(input);
		Assert.IsNotNull(result.Entries, "Command should be recognised");
		Assert.IsFalse(result.IsMatch, "Should not fully match with bad args");
	}

	[TestCase("TstI", TokenState.Partial)]
	[TestCase("Tst", TokenState.Partial)]
	[TestCase("TstN", TokenState.Partial)]
	[TestCase("Zzz", TokenState.Failed)]
	[TestCase("123cmd", TokenState.Failed)]
	public void Parse_CommandToken_HasExpectedState(string input, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(input).CommandToken.State);
	}

	[TestCase("TstInt 42", "TstInt")]
	[TestCase("TstNoArgs", "TstNoArgs")]
	[TestCase("UnknownCmd 1", "UnknownCmd")]
	[TestCase("Tst 1 2 3", "Tst")]
	public void Parse_CommandToken_RawIsCorrect(string input, string expectedRaw)
	{
		Assert.AreEqual(expectedRaw, _parser.Parse(input).CommandToken.Raw);
	}

	[TestCase("TstNoArgs", 0)]
	[TestCase("TstInt 42", 1)]
	[TestCase("TstBool false", 1)]
	[TestCase("TstFloats 1.0 2.0 3.0", 3)]
	public void Parse_Match_ArgTokenCount(string input, int expectedCount)
	{
		var result = _parser.Parse(input);
		Assert.IsTrue(result.IsMatch);
		Assert.AreEqual(expectedCount, result.ArgTokens.Count);
	}

	[TestCase("TstInt 42", 0)]
	[TestCase("TstBool true", 0)]
	[TestCase("TstFloats 1.0 2.0 3.0", 0)]
	[TestCase("TstFloats 1.0 2.0 3.0", 1)]
	[TestCase("TstFloats 1.0 2.0 3.0", 2)]
	[TestCase("TstOverload 1.0 2.0 3.0", 0)]
	public void Parse_Match_ArgToken_IsCompleted(string input, int argIndex)
	{
		var result = _parser.Parse(input);
		Assert.IsTrue(result.IsMatch);
		Assert.AreEqual(TokenState.Completed, result.ArgTokens[argIndex].State);
	}

	[TestCase("TstVec (1, 2, ")]
	[TestCase("TstBool tr")]
	[TestCase("TstBool tru")]
	[TestCase("TstIntList [1,2,")]
	[TestCase("TstBound >TstNoArgs")]
	[TestCase("TstBound >TstInt 4")]
	public void Parse_PartialArg_CommandRecognisedNoMatch(string input)
	{
		var result = _parser.Parse(input);
		Assert.IsNotNull(result.Entries, "Command should be recognised");
		Assert.IsFalse(result.IsMatch);
	}

	[Test]
	public void GetError_UnknownCommand_ContainsCommandName()
	{
		var result = _parser.Parse("XyzUnknown 42");
		StringAssert.Contains("XyzUnknown", result.GetError());
	}

	[Test]
	public void GetError_KnownCommandBadArgs_ContainsCommandName()
	{
		var result = _parser.Parse("TstInt abc");
		StringAssert.Contains("TstInt", result.GetError());
	}
}
}