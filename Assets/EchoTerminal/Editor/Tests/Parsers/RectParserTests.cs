using NUnit.Framework;
using UnityEngine;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class RectParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private RectParser _parser;

	[Test]
	public void Type_IsRect()
	{
		Assert.AreEqual(typeof(Rect), _parser.Type);
	}

	[TestCase("(0, 0, 100, 50)", TokenState.Resolved)]
	[TestCase("(10, 20, 300, 400)", TokenState.Resolved)]
	[TestCase("(-5, -10, 1.5, 2.5)", TokenState.Resolved)]
	[TestCase("(0, 0, 0, 0)", TokenState.Resolved)]
	[TestCase("(0, 0, 100", TokenState.Pending)]
	[TestCase("(", TokenState.Pending)]
	[TestCase("(0, 0, 100, )", TokenState.Invalid)]
	[TestCase("(abc, 0, 100, 50)", TokenState.Invalid)]
	[TestCase("(0, 0, 100)", TokenState.Invalid)]
	[TestCase("(0, 0, 100, 50, 0)", TokenState.Invalid)]
	[TestCase("(, , , )", TokenState.Invalid)]
	[TestCase("0, 0, 100, 50", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void RectParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[Test]
	public void RectParseValue_ReturnsExpectedRect()
	{
		Assert.AreEqual(new Rect(0f, 0f, 100f, 50f), _parser.ParseValue("(0, 0, 100, 50)"));
	}

	[Test]
	public void RectParseValue_WithDecimals_ReturnsExpectedRect()
	{
		Assert.AreEqual(new Rect(-5f, -10f, 1.5f, 2.5f), _parser.ParseValue("(-5, -10, 1.5, 2.5)"));
	}

	[Test]
	public void RectParseValue_Zero_ReturnsZeroRect()
	{
		Assert.AreEqual(new Rect(0f, 0f, 0f, 0f), _parser.ParseValue("(0, 0, 0, 0)"));
	}
}
}