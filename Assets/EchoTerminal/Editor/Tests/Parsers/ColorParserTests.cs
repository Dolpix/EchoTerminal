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
	public void ParseTokenState_Named_Completed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("r", TokenState.Partial)]
	[TestCase("re", TokenState.Partial)]
	[TestCase("gr", TokenState.Partial)]
	[TestCase("bl", TokenState.Partial)]
	[TestCase("wh", TokenState.Partial)]
	[TestCase("ma", TokenState.Partial)]
	[TestCase("cle", TokenState.Partial)]
	[TestCase("cy", TokenState.Partial)]
	public void ParseTokenState_Named_Partial(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("purple", TokenState.Failed)]
	[TestCase("orange", TokenState.Failed)]
	[TestCase("pink", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void ParseTokenState_Named_Failed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#FF0000", TokenState.Completed)]
	[TestCase("#00FF00", TokenState.Completed)]
	[TestCase("#0000FF", TokenState.Completed)]
	[TestCase("#FFFFFF", TokenState.Completed)]
	[TestCase("#000000", TokenState.Completed)]
	[TestCase("#ff0000", TokenState.Completed)]
	[TestCase("#FF0000FF", TokenState.Completed)]
	[TestCase("#00000080", TokenState.Completed)]
	public void ParseTokenState_Hex_Completed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#", TokenState.Partial)]
	[TestCase("#FF", TokenState.Partial)]
	[TestCase("#FF00", TokenState.Partial)]
	[TestCase("#FF000", TokenState.Partial)]
	[TestCase("#FF0000F", TokenState.Partial)]
	public void ParseTokenState_Hex_Partial(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#GGGGGG", TokenState.Failed)]
	[TestCase("#FF00ZZ", TokenState.Failed)]
	[TestCase("#FFFFFFFFF", TokenState.Failed)]
	public void ParseTokenState_Hex_Failed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(1,0,0)", TokenState.Completed)]
	[TestCase("(0,1,0)", TokenState.Completed)]
	[TestCase("(0,0,1)", TokenState.Completed)]
	[TestCase("(1,1,1)", TokenState.Completed)]
	[TestCase("(0,0,0,0)", TokenState.Completed)]
	[TestCase("(1,0,0,1)", TokenState.Completed)]
	[TestCase("(0.5,0.5,0.5)", TokenState.Completed)]
	public void ParseTokenState_Tuple_Completed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(", TokenState.Partial)]
	[TestCase("(1", TokenState.Partial)]
	[TestCase("(1,0", TokenState.Partial)]
	[TestCase("(1,0,0", TokenState.Partial)]
	public void ParseTokenState_Tuple_Partial(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(1,0)", TokenState.Failed)]
	[TestCase("(1,0,0,0,0)", TokenState.Failed)]
	[TestCase("(a,b,c)", TokenState.Failed)]
	[TestCase("(1,x,0)", TokenState.Failed)]
	public void ParseTokenState_Tuple_Failed(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void ParseValue_Red_ReturnsRedColor()
	{
		Assert.AreEqual(Color.red, _parser.ParseValue("red"));
	}

	[Test]
	public void ParseValue_HexRed_ReturnsRedColor()
	{
		Assert.AreEqual(new Color(1f, 0f, 0f, 1f), _parser.ParseValue("#FF0000"));
	}

	[Test]
	public void ParseValue_TupleRGB_ReturnsColorWithFullAlpha()
	{
		var result = (Color)_parser.ParseValue("(1,0,0)");
		Assert.AreEqual(new Color(1f, 0f, 0f, 1f), result);
	}

	[Test]
	public void ParseValue_TupleRGBA_ReturnsColorWithAlpha()
	{
		var result = (Color)_parser.ParseValue("(1,0,0,0.5)");
		Assert.AreEqual(new Color(1f, 0f, 0f, 0.5f), result);
	}

	[Test]
	public void ParseValue_White_ReturnsWhiteColor()
	{
		Assert.AreEqual(Color.white, _parser.ParseValue("white"));
	}

	[Test]
	public void ParseValue_EmptyString_ReturnsNull()
	{
		Assert.IsNull(_parser.ParseValue(""));
	}
}
}