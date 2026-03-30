using System.Linq;
using NUnit.Framework;
using EchoTerminal.TerminalCore;

namespace EchoTerminal.Editor.Tests.Parsers
{
[TestFixture]
public class CommandNameParserTests
{
	private CommandNameParser _parser;

	[SetUp]
	public void SetUp()
	{
		_parser = TestParsers.CreateAll().OfType<CommandNameParser>().First();
	}

	[Test]
	public void Type_IsCommandName()
	{
		Assert.AreEqual(typeof(CommandName), _parser.Type);
	}

	[TestCase("Teleport", TokenState.Resolved)]
	[TestCase("Spawn", TokenState.Resolved)]
	[TestCase("Kill", TokenState.Resolved)]
	[TestCase("Tele", TokenState.Unresolved)]
	[TestCase("teleport", TokenState.Unresolved)]
	[TestCase("Unknown", TokenState.Unresolved)]
	[TestCase("", TokenState.Unresolved)]
	public void CommandNameParse_ReturnsExpectedState(string raw, TokenState expected)
	{
		Assert.AreEqual(expected, _parser.Parse(raw));
	}

	[Test]
	public void CommandNameParse_EmptyRegistry_ReturnsUnresolved()
	{
		var empty = new CommandNameParser(System.Array.Empty<string>());
		Assert.AreEqual(TokenState.Unresolved, empty.Parse("Teleport"));
	}
}
}