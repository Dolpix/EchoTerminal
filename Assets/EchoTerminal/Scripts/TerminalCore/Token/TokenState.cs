public enum TokenState
{
	Unresolved, // Non-Identifiable signature
	Pending,    // Identifiable signature not finished
	Resolved,   // Identifiable signature finished
	Invalid     // Identifiable signature but incorrect as it's not in that data set. like a target name not existing.
}