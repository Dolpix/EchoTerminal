namespace EchoTerminal.Scripts.TerminalCore.Token
{
public enum TokenState
{
	Completed, // Recognised and finished
	Partial,   // Recognised and still being typed
	Failed     // Not recognised at all
}
}