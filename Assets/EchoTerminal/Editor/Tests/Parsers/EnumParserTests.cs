using System;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class EnumParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	public enum Direction
	{
		North,
		South,
		East,
		West,
		NorthEast,
		NorthWest
	}

	private EnumParser _parser;

	[Test]
	public void Type_IsEnum()
	{
		Assert.AreEqual(typeof(Enum), _parser.Type);
	}

	[TestCase("North")]
	[TestCase("Space")]
	[TestCase("Red")]
	public void ParseTokenState_NoExpectedType_ReturnsUnresolved(string raw)
	{
		Assert.AreEqual(TokenState.Unresolved, _parser.ParseTokenState(raw));
	}

	[TestCase("North", TokenState.Resolved)]
	[TestCase("south", TokenState.Resolved)]
	[TestCase("EAST", TokenState.Resolved)]
	[TestCase("NorthEast", TokenState.Resolved)]
	[TestCase("nor", TokenState.Pending)]
	[TestCase("No", TokenState.Pending)]
	[TestCase("northw", TokenState.Pending)]
	[TestCase("Up", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("Northe", TokenState.Pending)]
	public void ParseTokenState_Direction_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(Direction)));
	}

	[TestCase("North", Direction.North)]
	[TestCase("south", Direction.South)]
	[TestCase("EAST", Direction.East)]
	[TestCase("NorthEast", Direction.NorthEast)]
	public void ParseValue_Direction_ReturnsExpectedValue(string raw, Direction expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw, typeof(Direction)));
	}

	[TestCase("Space", TokenState.Resolved)]
	[TestCase("space", TokenState.Resolved)]
	[TestCase("SPACE", TokenState.Resolved)]
	[TestCase("A", TokenState.Resolved)]
	[TestCase("Enter", TokenState.Resolved)]
	[TestCase("Sp", TokenState.Pending)]
	[TestCase("Ent", TokenState.Pending)]
	[TestCase("Lef", TokenState.Pending)]
	[TestCase("ZZZ", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void ParseTokenState_Key_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(Key)));
	}

	[TestCase("Space", Key.Space)]
	[TestCase("space", Key.Space)]
	[TestCase("A", Key.A)]
	[TestCase("Enter", Key.Enter)]
	public void ParseValue_Key_ReturnsExpectedValue(string raw, Key expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw, typeof(Key)));
	}

	public enum PaletteColor
	{
		Red,
		Green,
		Blue,
		Yellow,
		Cyan,
		Magenta,
		White,
		Black
	}

	[TestCase("Red", TokenState.Resolved)]
	[TestCase("red", TokenState.Resolved)]
	[TestCase("GREEN", TokenState.Resolved)]
	[TestCase("Blue", TokenState.Resolved)]
	[TestCase("Bl", TokenState.Pending)]
	[TestCase("Mag", TokenState.Pending)]
	[TestCase("Wh", TokenState.Pending)]
	[TestCase("Purple", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void ParseTokenState_PaletteColor_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw, typeof(PaletteColor)));
	}

	[TestCase("Red", PaletteColor.Red)]
	[TestCase("green", PaletteColor.Green)]
	[TestCase("BLUE", PaletteColor.Blue)]
	[TestCase("Magenta", PaletteColor.Magenta)]
	public void ParseValue_PaletteColor_ReturnsExpectedValue(string raw, PaletteColor expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw, typeof(PaletteColor)));
	}

	[Test]
	public void ParseValue_NoExpectedType_ReturnsNull()
	{
		Assert.IsNull(_parser.ParseValue("North"));
	}

	[Test]
	public void ParseValue_NonEnumType_ReturnsNull()
	{
		Assert.IsNull(_parser.ParseValue("42", typeof(int)));
	}
}
}