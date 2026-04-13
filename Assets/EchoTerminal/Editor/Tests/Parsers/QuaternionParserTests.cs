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

	[Test]
	public void QuaternionParseValue_Identity_ReturnsIdentityQuaternion()
	{
		Assert.AreEqual(Quaternion.identity, _parser.ParseValue("(0, 0, 0, 1)"));
	}

	[Test]
	public void QuaternionParseValue_ReturnsExpectedQuaternion()
	{
		Assert.AreEqual(new Quaternion(0.5f, 0.5f, 0.5f, 0.5f), _parser.ParseValue("(0.5, 0.5, 0.5, 0.5)"));
	}

	[Test]
	public void QuaternionParseValue_Zero_ReturnsZeroQuaternion()
	{
		Assert.AreEqual(new Quaternion(0f, 0f, 0f, 0f), _parser.ParseValue("(0, 0, 0, 0)"));
	}
}
}