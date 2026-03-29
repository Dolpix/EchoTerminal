using NUnit.Framework;

[TestFixture]
public class StringParserTests
{
	[Test]
	public void Type_IsString()
	{
		Assert.AreEqual(typeof(string), new StringParser().Type);
	}

	[Test]
	public void NoValidValues_AnyLetterString_ReturnsResolved()
	{
		Assert.AreEqual(TokenState.Resolved, new StringParser().Parse("Goblin"));
	}

	[Test]
	public void WithValidValues_ExactMatch_ReturnsResolved()
	{
		var parser = new StringParser(new[] { "Goblin", "Dragon" });
		Assert.AreEqual(TokenState.Resolved, parser.Parse("Goblin"));
	}

	[Test]
	public void WithValidValues_PartialMatch_ReturnsUnresolved()
	{
		var parser = new StringParser(new[] { "Goblin", "Dragon" });
		Assert.AreEqual(TokenState.Unresolved, parser.Parse("Gob"));
	}

	[Test]
	public void WithValidValues_NoMatch_NoPrefixMatch_ReturnsInvalid()
	{
		var parser = new StringParser(new[] { "Goblin", "Dragon" });
		Assert.AreEqual(TokenState.Invalid, parser.Parse("Unknown"));
	}

	[Test]
	public void WithValidValues_NoPrefixMatch_ReturnsInvalid()
	{
		var parser = new StringParser(new[] { "Goblin", "Dragon" });
		Assert.AreEqual(TokenState.Invalid, parser.Parse("Xyz"));
	}

	[TestCase("123abc", TokenState.Unresolved)]
	[TestCase("1", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("@Player", TokenState.Unresolved)]
	public void NonLetterStart_ReturnsUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, new StringParser().Parse(raw));
	}
}