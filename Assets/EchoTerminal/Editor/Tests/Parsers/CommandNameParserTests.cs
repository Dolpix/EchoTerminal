using NUnit.Framework;

[TestFixture]
public class CommandNameParserTests
{
	private CommandNameParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = new(new[] { "Teleport", "Spawn", "Kill" });
	}

	[Test]
	public void Type_IsCommandName()
	{
		Assert.AreEqual(typeof(CommandName), _parser.Type);
	}

	[TestCase("Teleport", TokenState.Resolved)]
	[TestCase("Spawn", TokenState.Resolved)]
	[TestCase("Kill", TokenState.Resolved)]
	public void KnownCommands_ReturnResolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[TestCase("Tele", TokenState.Unresolved)]
	[TestCase("Unknown", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	[TestCase("teleport", TokenState.Unresolved)]
	public void UnknownOrPartialCommands_ReturnUnresolved(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[Test]
	public void EmptyRegistry_AnyInput_ReturnsUnresolved()
	{
		var emptyParser = new CommandNameParser(System.Array.Empty<string>());
		Assert.AreEqual(TokenState.Unresolved, emptyParser.Parse("Teleport"));
	}
}