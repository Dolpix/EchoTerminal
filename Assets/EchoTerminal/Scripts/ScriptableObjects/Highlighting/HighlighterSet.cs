using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EchoTerminal.Scripts.ScriptableObjects.Highlighting
{
[CreateAssetMenu(fileName = "HighlighterSet", menuName = "Echo Terminal/Highlighter Set")]
public class HighlighterSet : ScriptableObject
{
	[SerializeField] private TokenHighlighter _defaultHighlighter;
	[SerializeField] private List<HighlighterEntry> _entries = new();

	private Dictionary<Type, TokenHighlighter> _lookup;

	public TokenHighlighter DefaultHighlighter => _defaultHighlighter;
	public List<HighlighterEntry> Entries => _entries;

	private void OnEnable()
	{
		BuildLookup();
	}

	private void BuildLookup()
	{
		_lookup = new();
		foreach (HighlighterEntry entry in _entries)
		{
			if (string.IsNullOrEmpty(entry.TypeAssemblyQualifiedName) || entry.Highlighter == null)
			{
				continue;
			}

			Type type = Type.GetType(entry.TypeAssemblyQualifiedName) ?? ResolveByDisplayName(entry);
			if (type != null)
			{
				_lookup[type] = entry.Highlighter;
			}
		}
	}

	private static Type ResolveByDisplayName(HighlighterEntry entry)
	{
		if (string.IsNullOrEmpty(entry.TypeDisplayName))
		{
			return null;
		}

		string[] parts = entry.TypeAssemblyQualifiedName.Split(',');
		string assemblyName = parts.Length > 1 ? parts[1].Trim() : null;
		if (assemblyName == null)
		{
			return null;
		}

		Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
								.FirstOrDefault(a => a.GetName().Name == assemblyName);
		return asm?.GetTypes().FirstOrDefault(t => t.Name == entry.TypeDisplayName);
	}

	public bool TryGet(Type tokenType, out TokenHighlighter highlighter)
	{
		_lookup ??= new();

		if (_lookup.TryGetValue(tokenType, out highlighter))
		{
			return true;
		}

		for (Type t = tokenType.BaseType; t != null; t = t.BaseType)
		{
			if (_lookup.TryGetValue(t, out highlighter))
			{
				return true;
			}
		}

		foreach (Type face in tokenType.GetInterfaces())
		{
			if (_lookup.TryGetValue(face, out highlighter))
			{
				return true;
			}
		}

		highlighter = _defaultHighlighter;
		return highlighter != null;
	}

	public void AddEntry(Type type, TokenHighlighter highlighter)
	{
		_entries.Add(new()
		{
			TypeAssemblyQualifiedName = type.AssemblyQualifiedName,
			TypeDisplayName = type.Name,
			Highlighter = highlighter
		});
		BuildLookup();
	}

	public void RefreshLookup()
	{
		BuildLookup();
	}
}

[Serializable]
public class HighlighterEntry
{
	[HideInInspector] public string TypeAssemblyQualifiedName;
	[HideInInspector] public string TypeDisplayName;
	public TokenHighlighter Highlighter;
}
}