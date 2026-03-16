using System;
using System.Collections.Generic;

namespace EchoTerminal.Scripts.Test
{
public interface ISuggestor
{
	Type TargetType { get; }

	/// <summary>
	/// Return suggestions matching <paramref name="partial"/>.
	/// <paramref name="type"/> is the exact param type being filled — useful for generic
	/// suggestors like EnumSuggestor that handle a whole category of types.
	/// Filtering by partial is the implementor's responsibility.
	/// </summary>
	IReadOnlyList<string> GetSuggestions(Type type, string partial);
}
}