using EchoTerminal.Scripts.TerminalCore.Token;
using EchoTerminal.Scripts.TerminalCore.Token.TokenParser;
using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class Vec3ParserTests
{
	private Vec3Parser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	[Test]
	public void Type_IsVector3()
	{
		Assert.AreEqual(typeof(Vector3), _parser.Type);
	}

	[TestCase("(10, 0, 5)", TokenState.Completed)]
	[TestCase("(1.5, -2.0, 3.0)", TokenState.Completed)]
	[TestCase("(0, 0, 0)", TokenState.Completed)]
	[TestCase("(-1, -2, -3)", TokenState.Completed)]
	[TestCase("(10, 0", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(10, 0, )", TokenState.Failed)]
	[TestCase("(10, 0 )", TokenState.Failed)]
	[TestCase("(abc, 1, 2)", TokenState.Failed)]
	[TestCase("(1, 2)", TokenState.Failed)]
	[TestCase("(1, 2, 3, 4)", TokenState.Failed)]
	[TestCase("(, ,)", TokenState.Failed)]
	[TestCase("10, 0, 5", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void Vector3Parse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(10, 0, 5)", 10f, 0f, 5f)]
	[TestCase("(0, -1.5, 20)", 0f, -1.5f, 20f)]
	[TestCase("(0, 0, 0)", 0f, 0f, 0f)]
	[TestCase("(-2, -14, -2020)", -2f, -14f, -2020f)]
	[TestCase("(0.2, 0.001, 0.004)", 0.2f, 0.001f, 0.004f)]
	public void Vec3ParseValue_ReturnsExpectedVector3(string raw, float x, float y, float z)
	{
		var expected = new Vector3(x, y, z);
		var result = (Vector3)_parser.ParseValue(raw);

		Assert.AreEqual(expected.x, result.x, 0.001f);
		Assert.AreEqual(expected.y, result.y, 0.001f);
		Assert.AreEqual(expected.z, result.z, 0.001f);
	}
}
}