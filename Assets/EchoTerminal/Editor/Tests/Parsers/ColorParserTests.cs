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

	[TestCase("red", TokenState.Resolved)]
	[TestCase("RED", TokenState.Resolved)]
	[TestCase("Red", TokenState.Resolved)]
	[TestCase("green", TokenState.Resolved)]
	[TestCase("blue", TokenState.Resolved)]
	[TestCase("white", TokenState.Resolved)]
	[TestCase("black", TokenState.Resolved)]
	[TestCase("yellow", TokenState.Resolved)]
	[TestCase("cyan", TokenState.Resolved)]
	[TestCase("magenta", TokenState.Resolved)]
	[TestCase("clear", TokenState.Resolved)]
	[TestCase("grey", TokenState.Resolved)]
	[TestCase("gray", TokenState.Resolved)]
	public void ParseTokenState_Named_Resolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("r", TokenState.Pending)]
	[TestCase("re", TokenState.Pending)]
	[TestCase("gr", TokenState.Pending)]
	[TestCase("bl", TokenState.Pending)]
	[TestCase("wh", TokenState.Pending)]
	[TestCase("ma", TokenState.Pending)]
	[TestCase("cle", TokenState.Pending)]
	[TestCase("cy", TokenState.Pending)]
	public void ParseTokenState_Named_Pending(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("purple", TokenState.Unresolved)]
	[TestCase("orange", TokenState.Unresolved)]
	[TestCase("pink", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void ParseTokenState_Named_Unresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#FF0000", TokenState.Resolved)]
	[TestCase("#00FF00", TokenState.Resolved)]
	[TestCase("#0000FF", TokenState.Resolved)]
	[TestCase("#FFFFFF", TokenState.Resolved)]
	[TestCase("#000000", TokenState.Resolved)]
	[TestCase("#ff0000", TokenState.Resolved)]
	[TestCase("#FF0000FF", TokenState.Resolved)]
	[TestCase("#00000080", TokenState.Resolved)]
	public void ParseTokenState_Hex_Resolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#", TokenState.Pending)]
	[TestCase("#FF", TokenState.Pending)]
	[TestCase("#FF00", TokenState.Pending)]
	[TestCase("#FF000", TokenState.Pending)]
	[TestCase("#FF0000F", TokenState.Pending)]
	public void ParseTokenState_Hex_Pending(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("#GGGGGG", TokenState.Invalid)]
	[TestCase("#FF00ZZ", TokenState.Invalid)]
	[TestCase("#FFFFFFFFF", TokenState.Invalid)]
	public void ParseTokenState_Hex_Invalid(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(1,0,0)", TokenState.Resolved)]
	[TestCase("(0,1,0)", TokenState.Resolved)]
	[TestCase("(0,0,1)", TokenState.Resolved)]
	[TestCase("(1,1,1)", TokenState.Resolved)]
	[TestCase("(0,0,0,0)", TokenState.Resolved)]
	[TestCase("(1,0,0,1)", TokenState.Resolved)]
	[TestCase("(0.5,0.5,0.5)", TokenState.Resolved)]
	public void ParseTokenState_Tuple_Resolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(", TokenState.Pending)]
	[TestCase("(1", TokenState.Pending)]
	[TestCase("(1,0", TokenState.Pending)]
	[TestCase("(1,0,0", TokenState.Pending)]
	public void ParseTokenState_Tuple_Pending(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(1,0)", TokenState.Invalid)]
	[TestCase("(1,0,0,0,0)", TokenState.Invalid)]
	[TestCase("(a,b,c)", TokenState.Invalid)]
	[TestCase("(1,x,0)", TokenState.Invalid)]
	public void ParseTokenState_Tuple_Invalid(string raw, TokenState expected)
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