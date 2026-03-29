using NUnit.Framework;

[TestFixture]
public class IntParserTests
{
	private IntParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new IntParser();
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
	public void ValidIntegers_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[TestCase("3.14", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("1.0", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	public void InvalidIntegers_ReturnUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}