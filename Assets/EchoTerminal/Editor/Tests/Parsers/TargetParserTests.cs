using NUnit.Framework;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class TargetParserTests
{
	private TargetParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new(new[] { "@Player", "@Enemy1", "@Enemy2" });
	}

	[Test]
	public void Type_IsString()
	{
		Assert.AreEqual(typeof(string), _parser.Type);
	}

	[TestCase("@Player", TokenState.Resolved)]
	[TestCase("@Enemy1", TokenState.Resolved)]
	[TestCase("@Enemy2", TokenState.Resolved)]
	[TestCase("@P", TokenState.Unresolved)]
	[TestCase("@Unknown", TokenState.Invalid)]
	[TestCase("@XYZ", TokenState.Invalid)]
	[TestCase("@Z", TokenState.Invalid)]
	[TestCase("Player", TokenState.Unresolved)]
	[TestCase("abc", TokenState.Unresolved)]
	public void TargetParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}
}
}