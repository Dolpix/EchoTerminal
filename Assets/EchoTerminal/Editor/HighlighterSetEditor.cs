using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EchoTerminal.Scripts.ScriptableObjects.Highlighting;
using EchoTerminal.Scripts.ScriptableObjects.Highlighting.Highlighters;
using EchoTerminal.Scripts.TerminalCore.Token;
using UnityEditor;
using UnityEngine;

namespace EchoTerminal.Editor
{
[CustomEditor(typeof(HighlighterSet))]
public class HighlighterSetEditor : UnityEditor.Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		GUILayout.Space(8);

		if (GUILayout.Button("Auto-Generate Missing Entries", GUILayout.Height(28)))
		{
			AutoGenerateMissingEntries((HighlighterSet)target);
		}
	}

	private static void AutoGenerateMissingEntries(HighlighterSet set)
	{
		List<ITokenParser> parsers = ParserRegistry.CreateAllParsers();
		HashSet<string> existingNames = set.Entries
										   .Select(e => e.TypeAssemblyQualifiedName)
										   .ToHashSet();

		string setPath = AssetDatabase.GetAssetPath(set);
		string setDir = Path.GetDirectoryName(setPath);
		string highlightersDir = Path.Combine(setDir, "Highlighters");

		if (!AssetDatabase.IsValidFolder(highlightersDir))
		{
			AssetDatabase.CreateFolder(setDir, "Highlighters");
		}

		var added = 0;
		foreach (ITokenParser parser in parsers)
		{
			Type parserType = parser.Type;
			string qualifiedName = parserType.AssemblyQualifiedName;

			if (existingNames.Contains(qualifiedName))
			{
				continue;
			}

			var assetName = $"FlatColor_{parserType.Name}.asset";
			string assetPath = Path.Combine(highlightersDir, assetName).Replace("\\", "/");

			var existing = AssetDatabase.LoadAssetAtPath<FlatColorHighlighter>(assetPath);
			if (existing == null)
			{
				var highlighter = CreateInstance<FlatColorHighlighter>();
				AssetDatabase.CreateAsset(highlighter, assetPath);
				existing = highlighter;
			}

			set.Entries.Add(new()
			{
				TypeAssemblyQualifiedName = qualifiedName,
				TypeDisplayName = parserType.Name,
				Highlighter = existing
			});

			added++;
		}

		if (added > 0)
		{
			set.RefreshLookup();
			EditorUtility.SetDirty(set);
			AssetDatabase.SaveAssets();
			Debug.Log($"[HighlighterSet] Added {added} new entries.");
		}
		else
		{
			Debug.Log("[HighlighterSet] All parser types already have entries.");
		}
	}
}
}