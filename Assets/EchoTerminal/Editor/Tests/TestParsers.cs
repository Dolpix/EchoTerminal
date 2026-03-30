using EchoTerminal.TerminalCore;

namespace EchoTerminal.Editor.Tests
{
public static class TestParsers
{
	private static readonly string[] Commands = { "Teleport", "Spawn", "Kill" };
	private static readonly string[] Targets = { "@Player", "@Enemy1", "@Enemy2" };

	public static System.Collections.Generic.List<ITokenParser> CreateAll()
	{
		ParserRegistry.Register<CommandNameParser>(() => new CommandNameParser(Commands));
		ParserRegistry.Register<TargetParser>(() => new TargetParser(Targets));
		return ParserRegistry.CreateAll();
	}
}
}