using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
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

	[TestCase("(10, 0, 5)", TokenState.Completed)]
	[TestCase("(0, 0, 0)", TokenState.Completed)]
	[TestCase("(-1, -2, -3)", TokenState.Completed)]
	[TestCase("(100, 200, 300)", TokenState.Completed)]
	[TestCase("(10, 0", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(1.5, 2, 3)", TokenState.Failed)]
	[TestCase("(1, 2.0, 3)", TokenState.Failed)]
	[TestCase("(abc, 1, 2)", TokenState.Failed)]
	[TestCase("(1, 2)", TokenState.Failed)]
	[TestCase("(1, 2, 3, 4)", TokenState.Failed)]
	[TestCase("(10, 0, )", TokenState.Failed)]
	[TestCase("10, 0, 5", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void Vec3IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(10, 0, 5)", 10, 0, 5)]
	[TestCase("(-1, -2, -3)", -1, -2, -3)]
	[TestCase("(0, 0, 0)", 0, 0, 0)]
	[TestCase("(100, 200, 300)", 100, 200, 300)]
	[TestCase("(-50, 25, 0)", -50, 25, 0)]
	public void Vec3IntParseValue_ReturnsExpectedVector3Int(string raw, int x, int y, int z)
	{
		var expected = new Vector3Int(x, y, z);
		var result = (Vector3Int)_parser.ParseValue(raw);
		Assert.AreEqual(expected, result);
	}
}
}