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

	[TestCase("(0, 0, 0, 1)", TokenState.Resolved)]
	[TestCase("(0.5, 0.5, 0.5, 0.5)", TokenState.Resolved)]
	[TestCase("(-1, 0, 0, 1)", TokenState.Resolved)]
	[TestCase("(0, 0, 0, 0)", TokenState.Resolved)]
	[TestCase("(0, 0, 0", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(0, 0, 0, )", TokenState.Invalid)]
	[TestCase("(abc, 0, 0, 1)", TokenState.Invalid)]
	[TestCase("(0, 0, 0)", TokenState.Invalid)]
	[TestCase("(0, 0, 0, 1, 0)", TokenState.Invalid)]
	[TestCase("(, , , )", TokenState.Invalid)]
	[TestCase("0, 0, 0, 1", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
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