using System.Linq;
using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class IntParserTests
{
	private IntParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = TestParsers.CreateAll().OfType<IntParser>().First();
	}

	[Test]
	public void Type_IsInt()
	{
		Assert.AreEqual(typeof(int), _parser.Type);
	}

	[TestCase("42", TokenState.Resolved)]
	[TestCase("0", TokenState.Resolved)]
	[TestCase("-10", TokenState.Resolved)]
	[TestCase("999999", TokenState.Resolved)]
	[TestCase("3.14", TokenState.Unresolved)]
	[TestCase("1.0", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void IntParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}
}