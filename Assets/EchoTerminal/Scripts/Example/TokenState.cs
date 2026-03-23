public enum TokenState
{
	Unresolved, // Not mine
	Pending,    // Mine, still reading — don't split
	Resolved,   // Mine, complete, valid
	Invalid     // Mine, complete, broken
}