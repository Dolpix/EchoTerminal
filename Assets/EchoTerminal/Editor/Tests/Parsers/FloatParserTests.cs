using System.Linq;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class FloatParserTests
{
	private FloatParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = TestParsers.CreateAll().OfType<FloatParser>().First();
	}

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
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}
}