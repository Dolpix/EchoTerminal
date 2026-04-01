using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class FloatParserTests
{
	[SetUp]
	public void SetUp()
	{
		_parser = new();
	}

	private FloatParser _parser;

	[Test]
	public void Type_IsFloat()
	{
		Assert.AreEqual(typeof(float), _parser.Type);
	}

	[TestCase("3.14", TokenState.Resolved)]
	[TestCase("42", TokenState.Resolved)]
	[TestCase("-1.5", TokenState.Resolved)]
	[TestCase("0.0", TokenState.Resolved)]
	[TestCase("0", TokenState.Resolved)]
	[TestCase("1.2.3", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void FloatParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.ParseTokenState(raw));
	}

	[TestCase("3.14", 3.14f)]
	[TestCase("42", 42f)]
	[TestCase("-1.5", -1.5f)]
	[TestCase("0.0", 0.0f)]
	[TestCase("0", 0f)]
	public void FloatParseValue_ReturnsExpectedFloat(string raw, float expected)
	{
		Assert.AreEqual(expected, _parser.ParseValue(raw));
	}
}
}