using NUnit.Framework;

[TestFixture]
public class TargetParserTests
{
	private TargetParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new TargetParser(new[] { "@Player", "@Enemy1", "@Enemy2" });
	}

	[Test]
	public void Type_IsString()
	{
		Assert.AreEqual(typeof(string), _parser.Type);
	}

	[TestCase("@Player", TokenState.Resolved)]
	[TestCase("@Enemy1", TokenState.Resolved)]
	[TestCase("@Enemy2", TokenState.Resolved)]
	public void KnownTargets_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[TestCase("@Unknown", TokenState.Invalid)]
	[TestCase("@XYZ", TokenState.Invalid)]
	public void UnknownTargets_NoPrefixMatch_ReturnInvalid(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[Test]
	public void PartialTarget_WithPrefixMatch_ReturnsUnresolved()
	{
		Assert.AreEqual(TokenState.Unresolved, _parser.Parse("@P"));
	}

	[Test]
	public void PartialTarget_NoPrefixMatch_ReturnsInvalid()
	{
		Assert.AreEqual(TokenState.Invalid, _parser.Parse("@Z"));
	}

	[TestCase("Player", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	public void NoAtPrefix_ReturnsUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}