using NUnit.Framework;

[TestFixture]
public class FloatParserTests
{
	private FloatParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new FloatParser();
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
	public void ValidFloats_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("1.2.3", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	public void InvalidFloats_ReturnUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}