using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class QuaternionParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private QuaternionParser _parser;

	[Test]
	public void Type_IsQuaternion()
	{
		Assert.AreEqual(typeof(Quaternion), _parser.Type);
	}

	[TestCase("(0, 0, 0, 1)", TokenState.Completed)]
	[TestCase("(0.5, 0.5, 0.5, 0.5)", TokenState.Completed)]
	[TestCase("(-1, 0, 0, 1)", TokenState.Completed)]
	[TestCase("(0, 0, 0, 0)", TokenState.Completed)]
	[TestCase("(0, 0, 0", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(0, 0, 0, )", TokenState.Failed)]
	[TestCase("(abc, 0, 0, 1)", TokenState.Failed)]
	[TestCase("(0, 0, 0)", TokenState.Failed)]
	[TestCase("(0, 0, 0, 1, 0)", TokenState.Failed)]
	[TestCase("(, , , )", TokenState.Failed)]
	[TestCase("0, 0, 0, 1", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void QuaternionParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(0, 0, 0, 1)", 0f, 0f, 0f, 1f)]
	[TestCase("(0.5, 0.5, 0.5, 0.5)", 0.5f, 0.5f, 0.5f, 0.5f)]
	[TestCase("(0, 0, 0, 0)", 0f, 0f, 0f, 0f)]
	[TestCase("(-0.707, 0, 0, 0.707)", -0.707f, 0f, 0f, 0.707f)]
	public void QuaternionParseValue_ReturnsExpectedQuaternion(string raw, float x, float y, float z, float w)
	{
		var expected = new Quaternion(x, y, z, w);
		var result = (Quaternion)_parser.ParseValue(raw);

		Assert.AreEqual(expected.x, result.x, 0.001f);
		Assert.AreEqual(expected.y, result.y, 0.001f);
		Assert.AreEqual(expected.z, result.z, 0.001f);
		Assert.AreEqual(expected.w, result.w, 0.001f);
	}
}
}