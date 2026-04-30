using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec2ParserTests
{
	private Vec2Parser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	[Test]
	public void Type_IsVector2()
	{
		Assert.AreEqual(typeof(Vector2), _parser.Type);
	}

	[TestCase("(10, 0)", TokenState.Completed)]
	[TestCase("(1.5, -2.0)", TokenState.Completed)]
	[TestCase("(0, 0)", TokenState.Completed)]
	[TestCase("(-1, -2)", TokenState.Completed)]
	[TestCase("(10, 0", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(10, )", TokenState.Failed)]
	[TestCase("(abc, 1)", TokenState.Failed)]
	[TestCase("(1, 2, 3)", TokenState.Failed)]
	[TestCase("(, )", TokenState.Failed)]
	[TestCase("10, 0", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void Vec2Parse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(10, 0)", 10f, 0f)]
	[TestCase("(1.5, -2.0)", 1.5f, -2.0f)]
	[TestCase("(0, 0)", 0f, 0f)]
	[TestCase("(-1, -2)", -1f, -2f)]
	[TestCase("(0.005, 100.1)", 0.005f, 100.1f)]
	public void Vec2ParseValue_ReturnsExpectedVector2(string raw, float x, float y)
	{
		var expected = new Vector2(x, y);
		var result = (Vector2)_parser.ParseValue(raw);

		Assert.AreEqual(expected.x, result.x, 0.001f);
		Assert.AreEqual(expected.y, result.y, 0.001f);
	}
}
}