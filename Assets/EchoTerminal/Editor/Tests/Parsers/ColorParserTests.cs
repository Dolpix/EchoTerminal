using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class ColorParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private ColorParser _parser;

	[Test]
	public void Type_IsColor()
	{
		Assert.AreEqual(typeof(Color), _parser.Type);
	}

	// Named - Completed
	[TestCase("red", TokenState.Completed)]
	[TestCase("RED", TokenState.Completed)]
	[TestCase("Red", TokenState.Completed)]
	[TestCase("green", TokenState.Completed)]
	[TestCase("blue", TokenState.Completed)]
	[TestCase("white", TokenState.Completed)]
	[TestCase("black", TokenState.Completed)]
	[TestCase("yellow", TokenState.Completed)]
	[TestCase("cyan", TokenState.Completed)]
	[TestCase("magenta", TokenState.Completed)]
	[TestCase("clear", TokenState.Completed)]
	[TestCase("grey", TokenState.Completed)]
	[TestCase("gray", TokenState.Completed)]
	// Named - Partial
	[TestCase("r", TokenState.Partial)]
	[TestCase("re", TokenState.Partial)]
	[TestCase("RE", TokenState.Partial)]
	[TestCase("gr", TokenState.Partial)]
	[TestCase("gre", TokenState.Partial)]
	[TestCase("bl", TokenState.Partial)]
	[TestCase("bla", TokenState.Partial)]
	[TestCase("wh", TokenState.Partial)]
	[TestCase("whi", TokenState.Partial)]
	[TestCase("ma", TokenState.Partial)]
	[TestCase("cle", TokenState.Partial)]
	[TestCase("cy", TokenState.Partial)]
	[TestCase("ye", TokenState.Partial)]
	// Named - Failed (unsupported, misspelled, spaces, junk)
	[TestCase("", TokenState.Failed)]
	[TestCase("purple", TokenState.Failed)]
	[TestCase("orange", TokenState.Failed)]
	[TestCase("pink", TokenState.Failed)]
	[TestCase("redd", TokenState.Failed)]
	[TestCase("greens", TokenState.Failed)]
	[TestCase("red ", TokenState.Failed)]
	[TestCase(" red", TokenState.Failed)]
	[TestCase("re d", TokenState.Failed)]
	[TestCase("r3d", TokenState.Failed)]
	[TestCase("123", TokenState.Failed)]
	// Hex - Completed
	[TestCase("#FF0000", TokenState.Completed)]
	[TestCase("#00FF00", TokenState.Completed)]
	[TestCase("#0000FF", TokenState.Completed)]
	[TestCase("#FFFFFF", TokenState.Completed)]
	[TestCase("#000000", TokenState.Completed)]
	[TestCase("#ff0000", TokenState.Completed)]
	[TestCase("#aabbcc", TokenState.Completed)]
	[TestCase("#FF0000FF", TokenState.Completed)]
	[TestCase("#00000080", TokenState.Completed)]
	[TestCase("#AABBCCDD", TokenState.Completed)]
	// Hex - Partial (valid chars, incomplete length)
	[TestCase("#", TokenState.Partial)]
	[TestCase("#FF", TokenState.Partial)]
	[TestCase("#FF00", TokenState.Partial)]
	[TestCase("#FF000", TokenState.Partial)]
	[TestCase("#FF0000F", TokenState.Partial)]
	[TestCase("#abc", TokenState.Partial)]
	[TestCase("#abcde", TokenState.Partial)]
	// Hex - Failed (invalid chars, bad length, spaces, double hash)
	[TestCase("#GGGGGG", TokenState.Failed)]
	[TestCase("#FF00ZZ", TokenState.Failed)]
	[TestCase("#FFFFFFFFF", TokenState.Failed)]
	[TestCase("#FF000000X", TokenState.Failed)]
	[TestCase("#FF 0000", TokenState.Failed)]
	[TestCase("##FF0000", TokenState.Failed)]
	// Tuple - Completed (vec3 and vec4, with and without spaces)
	[TestCase("(1,0,0)", TokenState.Completed)]
	[TestCase("(0,1,0)", TokenState.Completed)]
	[TestCase("(0,0,1)", TokenState.Completed)]
	[TestCase("(0,0,0)", TokenState.Completed)]
	[TestCase("(1,1,1)", TokenState.Completed)]
	[TestCase("(0,0,0,0)", TokenState.Completed)]
	[TestCase("(1,0,0,1)", TokenState.Completed)]
	[TestCase("(0.5,0.5,0.5)", TokenState.Completed)]
	[TestCase("(0.5,0.5,0.5,0.5)", TokenState.Completed)]
	[TestCase("(1, 0, 0)", TokenState.Completed)]
	[TestCase("(2,0,0)", TokenState.Completed)]
	[TestCase("(-1,0,0)", TokenState.Completed)]
	// Tuple - Partial (opening paren, incomplete components)
	[TestCase("(", TokenState.Partial)]
	[TestCase("(1", TokenState.Partial)]
	[TestCase("(1,0", TokenState.Partial)]
	[TestCase("(1,0,0", TokenState.Partial)]
	[TestCase("(1,0,0,", TokenState.Partial)]
	[TestCase("(1.5,0", TokenState.Partial)]
	[TestCase("(0.5", TokenState.Partial)]
	// Tuple - Failed (wrong count, bad chars, missing parens, spaces in wrong place)
	[TestCase("(1,0)", TokenState.Failed)]
	[TestCase("(1,0,0,0,0)", TokenState.Failed)]
	[TestCase("(a,b,c)", TokenState.Failed)]
	[TestCase("(1,x,0)", TokenState.Failed)]
	[TestCase("(,0,0)", TokenState.Failed)]
	[TestCase("(1,,0)", TokenState.Failed)]
	[TestCase("( )", TokenState.Failed)]
	[TestCase("1,0,0", TokenState.Failed)]
	public void ParseTokenState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("red", 1f, 0f, 0f, 1f)]
	[TestCase("RED", 1f, 0f, 0f, 1f)]
	[TestCase("white", 1f, 1f, 1f, 1f)]
	[TestCase("black", 0f, 0f, 0f, 1f)]
	[TestCase("blue", 0f, 0f, 1f, 1f)]
	[TestCase("cyan", 0f, 1f, 1f, 1f)]
	[TestCase("magenta", 1f, 0f, 1f, 1f)]
	[TestCase("yellow", 1f, 1f, 0f, 1f)]
	[TestCase("#FF0000", 1f, 0f, 0f, 1f)]
	[TestCase("#00FF00", 0f, 1f, 0f, 1f)]
	[TestCase("#0000FF", 0f, 0f, 1f, 1f)]
	[TestCase("#FFFFFF", 1f, 1f, 1f, 1f)]
	[TestCase("#000000", 0f, 0f, 0f, 1f)]
	[TestCase("#ff0000", 1f, 0f, 0f, 1f)]
	[TestCase("#FF0000FF", 1f, 0f, 0f, 1f)]
	[TestCase("#FF000000", 1f, 0f, 0f, 0f)]
	[TestCase("(1,0,0)", 1f, 0f, 0f, 1f)]
	[TestCase("(0,1,0)", 0f, 1f, 0f, 1f)]
	[TestCase("(0,0,1)", 0f, 0f, 1f, 1f)]
	[TestCase("(1,0,0,0.5)", 1f, 0f, 0f, 0.5f)]
	[TestCase("(0.5,0.5,0.5)", 0.5f, 0.5f, 0.5f, 1f)]
	[TestCase("(0.5,0.5,0.5,0)", 0.5f, 0.5f, 0.5f, 0f)]
	[TestCase("(1, 0, 0)", 1f, 0f, 0f, 1f)]
	public void ParseValue_Valid(string raw, float r, float g, float b, float a)
	{
		var result = (Color)_parser.ParseValue(raw);
		Assert.AreEqual(new Color(r, g, b, a), result);
	}

	[TestCase("")]
	[TestCase("purple")]
	[TestCase("orange")]
	[TestCase("pink")]
	[TestCase("red ")]
	[TestCase(" red")]
	[TestCase("#GGGGGG")]
	[TestCase("#FF00ZZ")]
	[TestCase("1,0,0")]
	[TestCase("notacolor")]
	[TestCase("123")]
	public void ParseValue_Invalid_ReturnsNull(string raw)
	{
		Assert.IsNull(_parser.ParseValue(raw));
	}
}
}