namespace EchoTerminal
{
public enum TokenState
{
	Unresolved, // Not my format, or format matches but not yet complete
	Pending,    // My format, still reading — don't split on spaces
	Resolved,   // My format, complete and valid
	Invalid     // My format, complete but broken
}
}
