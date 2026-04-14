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

	[TestCase("(10, 0)", TokenState.Completed)]
	[TestCase("(0, 0)", TokenState.Completed)]
	[TestCase("(-1, -2)", TokenState.Completed)]
	[TestCase("(100, 200)", TokenState.Completed)]
	[TestCase("(10, 0", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(1.5, 2)", TokenState.Failed)]
	[TestCase("(1, 2.0)", TokenState.Failed)]
	[TestCase("(abc, 1)", TokenState.Failed)]
	[TestCase("(1, 2, 3)", TokenState.Failed)]
	[TestCase("(10, )", TokenState.Failed)]
	[TestCase("10, 0", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void Vec2IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(10, 0)", 10, 0)]
	[TestCase("(-1, -2)", -1, -2)]
	[TestCase("(0, 0)", 0, 0)]
	[TestCase("(100, 200)", 100, 200)]
	[TestCase("(-50, 25)", -50, 25)]
	public void Vec2IntParseValue_ReturnsExpectedVector2Int(string raw, int x, int y)
	{
		var expected = new Vector2Int(x, y);
		var result = (Vector2Int)_parser.ParseValue(raw);
		Assert.AreEqual(expected, result);
	}
}
}