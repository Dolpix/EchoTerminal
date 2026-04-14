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

	[TestCase("(0, 0, 100, 50)", TokenState.Completed)]
	[TestCase("(10, 20, 300, 400)", TokenState.Completed)]
	[TestCase("(-5, -10, 1.5, 2.5)", TokenState.Completed)]
	[TestCase("(0, 0, 0, 0)", TokenState.Completed)]
	[TestCase("(0, 0, 100", TokenState.Partial)]
	[TestCase("(", TokenState.Partial)]
	[TestCase("(0, 0, 100, )", TokenState.Failed)]
	[TestCase("(abc, 0, 100, 50)", TokenState.Failed)]
	[TestCase("(0, 0, 100)", TokenState.Failed)]
	[TestCase("(0, 0, 100, 50, 0)", TokenState.Failed)]
	[TestCase("(, , , )", TokenState.Failed)]
	[TestCase("0, 0, 100, 50", TokenState.Failed)]
	[TestCase("abc", TokenState.Failed)]
	[TestCase("", TokenState.Failed)]
	public void RectParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("(0, 0, 100, 50)", 0f, 0f, 100f, 50f)]
	[TestCase("(-5, -10, 1.5, 2.5)", -5f, -10f, 1.5f, 2.5f)]
	[TestCase("(0, 0, 0, 0)", 0f, 0f, 0f, 0f)]
	[TestCase("(10.5, 20.5, 30.5, 40.5)", 10.5f, 20.5f, 30.5f, 40.5f)]
	public void RectParseValue_ReturnsExpectedRect(string raw, float x, float y, float w, float h)
	{
		var expected = new Rect(x, y, w, h);
		var result = (Rect)_parser.ParseValue(raw);
		Assert.AreEqual(expected.x, result.x, 0.001f);
		Assert.AreEqual(expected.y, result.y, 0.001f);
		Assert.AreEqual(expected.width, result.width, 0.001f);
		Assert.AreEqual(expected.height, result.height, 0.001f);
	}
}
}