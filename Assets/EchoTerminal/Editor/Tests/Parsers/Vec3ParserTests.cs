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

	[Test]
	public void Vec3ParseValue_ReturnsExpectedVector3()
	{
		Assert.AreEqual(new Vector3(10f, 0f, 5f), _parser.ParseValue("(10, 0, 5)"));
	}

	[Test]
	public void Vec3ParseValue_WithNegativesAndDecimals_ReturnsExpectedVector3()
	{
		Assert.AreEqual(new Vector3(1.5f, -2.0f, 3.0f), _parser.ParseValue("(1.5, -2.0, 3.0)"));
	}

	[Test]
	public void Vec3ParseValue_Zero_ReturnsZeroVector()
	{
		Assert.AreEqual(Vector3.zero, _parser.ParseValue("(0, 0, 0)"));
	}
}
}