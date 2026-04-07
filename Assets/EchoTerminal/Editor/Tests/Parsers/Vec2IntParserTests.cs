using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec2IntParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private Vec2IntParser _parser;

	[Test]
	public void Type_IsVector2Int()
	{
		Assert.AreEqual(typeof(Vector2Int), _parser.Type);
	}

	[TestCase("(10, 0)", TokenState.Resolved)]
	[TestCase("(0, 0)", TokenState.Resolved)]
	[TestCase("(-1, -2)", TokenState.Resolved)]
	[TestCase("(100, 200)", TokenState.Resolved)]
	[TestCase("(10, 0", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(1.5, 2)", TokenState.Invalid)]
	[TestCase("(1, 2.0)", TokenState.Invalid)]
	[TestCase("(abc, 1)", TokenState.Invalid)]
	[TestCase("(1, 2, 3)", TokenState.Invalid)]
	[TestCase("(10, )", TokenState.Invalid)]
	[TestCase("10, 0", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void Vec2IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void Vec2IntParseValue_ReturnsExpectedVector2Int()
	{
		Assert.AreEqual(new Vector2Int(10, 0), _parser.ParseValue("(10, 0)"));
	}

	[Test]
	public void Vec2IntParseValue_WithNegatives_ReturnsExpectedVector2Int()
	{
		Assert.AreEqual(new Vector2Int(-1, -2), _parser.ParseValue("(-1, -2)"));
	}

	[Test]
	public void Vec2IntParseValue_Zero_ReturnsZeroVector()
	{
		Assert.AreEqual(Vector2Int.zero, _parser.ParseValue("(0, 0)"));
	}
}
}