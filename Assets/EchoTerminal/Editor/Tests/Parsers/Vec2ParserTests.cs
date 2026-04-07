using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec2ParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private Vec2Parser _parser;

	[Test]
	public void Type_IsVector2()
	{
		Assert.AreEqual(typeof(Vector2), _parser.Type);
	}

	[TestCase("(10, 0)", TokenState.Resolved)]
	[TestCase("(1.5, -2.0)", TokenState.Resolved)]
	[TestCase("(0, 0)", TokenState.Resolved)]
	[TestCase("(-1, -2)", TokenState.Resolved)]
	[TestCase("(10, 0", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(10, )", TokenState.Invalid)]
	[TestCase("(abc, 1)", TokenState.Invalid)]
	[TestCase("(1, 2, 3)", TokenState.Invalid)]
	[TestCase("(, )", TokenState.Invalid)]
	[TestCase("10, 0", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void Vec2Parse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void Vec2ParseValue_ReturnsExpectedVector2()
	{
		Assert.AreEqual(new Vector2(10f, 0f), _parser.ParseValue("(10, 0)"));
	}

	[Test]
	public void Vec2ParseValue_WithNegativesAndDecimals_ReturnsExpectedVector2()
	{
		Assert.AreEqual(new Vector2(1.5f, -2.0f), _parser.ParseValue("(1.5, -2.0)"));
	}

	[Test]
	public void Vec2ParseValue_Zero_ReturnsZeroVector()
	{
		Assert.AreEqual(Vector2.zero, _parser.ParseValue("(0, 0)"));
	}
}
}