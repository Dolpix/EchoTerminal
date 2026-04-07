using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec3IntParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private Vec3IntParser _parser;

	[Test]
	public void Type_IsVector3Int()
	{
		Assert.AreEqual(typeof(Vector3Int), _parser.Type);
	}

	[TestCase("(10, 0, 5)", TokenState.Resolved)]
	[TestCase("(0, 0, 0)", TokenState.Resolved)]
	[TestCase("(-1, -2, -3)", TokenState.Resolved)]
	[TestCase("(100, 200, 300)", TokenState.Resolved)]
	[TestCase("(10, 0", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(1.5, 2, 3)", TokenState.Invalid)]
	[TestCase("(1, 2.0, 3)", TokenState.Invalid)]
	[TestCase("(abc, 1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2)", TokenState.Invalid)]
	[TestCase("(1, 2, 3, 4)", TokenState.Invalid)]
	[TestCase("(10, 0, )", TokenState.Invalid)]
	[TestCase("10, 0, 5", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void Vec3IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void Vec3IntParseValue_ReturnsExpectedVector3Int()
	{
		Assert.AreEqual(new Vector3Int(10, 0, 5), _parser.ParseValue("(10, 0, 5)"));
	}

	[Test]
	public void Vec3IntParseValue_WithNegatives_ReturnsExpectedVector3Int()
	{
		Assert.AreEqual(new Vector3Int(-1, -2, -3), _parser.ParseValue("(-1, -2, -3)"));
	}

	[Test]
	public void Vec3IntParseValue_Zero_ReturnsZeroVector()
	{
		Assert.AreEqual(Vector3Int.zero, _parser.ParseValue("(0, 0, 0)"));
	}
}
}