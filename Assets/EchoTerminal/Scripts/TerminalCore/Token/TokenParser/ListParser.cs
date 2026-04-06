using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ListParser : ITokenParser
{
	private readonly IList<ITokenParser> _elementParsers;

	public ListParser(IList<ITokenParser> elementParsers)
	{
		_elementParsers = elementParsers;
	}

	public Type Type => typeof(IList);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '[')
		{
			return TokenState.Unresolved;
		}

		if (!IsBalanced(raw))
		{
			return TokenState.Pending;
		}

		var inner = raw[1..^1];

		if (string.IsNullOrWhiteSpace(inner))
		{
			return TokenState.Resolved;
		}

		var elementParser = GetElementParser(expectedType);

		if (elementParser == null)
		{
			return _elementParsers.Any(ep => AllElementsResolve(inner, ep))
				? TokenState.Resolved
				: TokenState.Invalid;
		}

		var elements = SplitElements(inner, elementParser);

		if (elements == null)
		{
			return TokenState.Invalid;
		}

		return elements.All(e => elementParser.ParseTokenState(e) == TokenState.Resolved)
			? TokenState.Resolved
			: TokenState.Invalid;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		var inner = raw[1..^1];
		var elementParser = GetElementParser(expectedType);

		if (elementParser == null)
		{
			if (string.IsNullOrWhiteSpace(inner))
			{
				return new List<object>();
			}

			foreach (var ep in _elementParsers)
			{
				var elements = SplitElements(inner, ep);

				if (elements == null || elements.Any(e => ep.ParseTokenState(e) != TokenState.Resolved))
				{
					continue;
				}

				var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(ep.Type));

				foreach (var e in elements)
				{
					list.Add(ep.ParseValue(e));
				}

				return list;
			}

			return new List<object>();
		}

		if (string.IsNullOrWhiteSpace(inner))
		{
			return CreateEmptyList(expectedType);
		}

		var splitElements = SplitElements(inner, elementParser);

		if (splitElements == null)
		{
			return CreateEmptyList(expectedType);
		}

		var result = CreateEmptyList(expectedType);

		foreach (var e in splitElements)
		{
			result.Add(elementParser.ParseValue(e));
		}

		return result;
	}

	private bool AllElementsResolve(string inner, ITokenParser elementParser)
	{
		var elements = SplitElements(inner, elementParser);
		return elements != null && elements.All(e => elementParser.ParseTokenState(e) == TokenState.Resolved);
	}

	private ITokenParser GetElementParser(Type listType)
	{
		var elementType = GetElementType(listType);

		if (elementType == null)
		{
			return null;
		}

		var exact = _elementParsers.FirstOrDefault(p => p.Type == elementType);

		if (exact != null)
		{
			return exact;
		}

		if (!elementType.IsGenericType)
		{
			return null;
		}

		var contextual = _elementParsers.OfType<ListParser>().FirstOrDefault();

		return contextual != null ? new BoundParser(contextual, elementType) : null;
	}

	private static Type GetElementType(Type listType)
	{
		if (listType == null || !listType.IsGenericType)
		{
			return null;
		}

		return listType.GetGenericArguments().FirstOrDefault();
	}

	private static IList CreateEmptyList(Type listType)
	{
		var elementType = GetElementType(listType) ?? typeof(object);
		return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
	}

	private static bool IsBalanced(string raw)
	{
		var depth = 0;
		var inQuote = false;

		foreach (var c in raw)
		{
			if (c == '"')
			{
				inQuote = !inQuote;
			}
			else if (!inQuote && c == '[')
			{
				depth++;
			}
			else if (!inQuote && c == ']')
			{
				depth--;
				if (depth < 0)
				{
					return false;
				}
			}
		}

		return depth == 0;
	}

	private static List<string> SplitElements(string inner, ITokenParser elementParser)
	{
		var parts = new List<string>();
		var i = 0;

		while (i < inner.Length)
		{
			while (i < inner.Length && char.IsWhiteSpace(inner[i]))
			{
				i++;
			}

			if (i >= inner.Length)
			{
				break;
			}

			var start = i;
			var found = false;

			while (i <= inner.Length)
			{
				if (i > start)
				{
					var candidate = inner[start..i].Trim();

					if (elementParser.ParseTokenState(candidate) == TokenState.Resolved)
					{
						var j = i;

						while (j < inner.Length && char.IsWhiteSpace(inner[j]))
						{
							j++;
						}

						if (j >= inner.Length || inner[j] == ',')
						{
							parts.Add(candidate);
							i = j < inner.Length ? j + 1 : inner.Length;
							found = true;
							break;
						}
					}
				}

				if (i >= inner.Length)
				{
					break;
				}

				i++;
			}

			if (!found)
			{
				return null;
			}
		}

		return parts;
	}

	private sealed class BoundParser : ITokenParser
	{
		private readonly ITokenParser _inner;

		public BoundParser(ITokenParser inner, Type boundType)
		{
			_inner = inner;
			Type = boundType;
		}

		public Type Type { get; }

		public TokenState ParseTokenState(string raw, System.Type expectedType = null)
		{
			return _inner.ParseTokenState(raw, Type);
		}

		public object ParseValue(string raw, System.Type expectedType = null)
		{
			return _inner.ParseValue(raw, Type);
		}
	}
}