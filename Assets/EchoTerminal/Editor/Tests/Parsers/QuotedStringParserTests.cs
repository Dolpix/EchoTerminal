using NUnit.Framework;

[TestFixture]
public class QuotedStringParserTests
{
	private QuotedStringParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new QuotedStringParser();
	}

	[Test]
	public void Type_IsString()
	{
		Assert.AreEqual(typeof(string), _parser.Type);
	}

	[TestCase("\"Hello World\"", TokenState.Resolved)]
	[TestCase("\"Hello\"", TokenState.Resolved)]
	[TestCase("\"\"", TokenState.Resolved)]
	[TestCase("\"a b c d\"", TokenState.Resolved)]
	public void ClosedQuotedStrings_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[Test]
	public void OpenQuote_ReturnsPending()
	{
		Assert.AreEqual(TokenState.Pending, _parser.Parse("\"Hello"));
	}

	[Test]
	public void SingleQuoteChar_ReturnsPending()
	{
		Assert.AreEqual(TokenState.Pending, _parser.Parse("\""));
	}

	[TestCase("Hello", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("42", TokenState.Unresolved)]
	public void NoLeadingQuote_ReturnsUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}