using UnityEditor;
using UnityEngine;

namespace EchoTerminal.Editor
{
[CustomPropertyDrawer(typeof(HighlighterEntry))]
public class HighlighterEntryDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SerializedProperty displayName = property.FindPropertyRelative("TypeDisplayName");
		SerializedProperty highlighter = property.FindPropertyRelative("Highlighter");

		string labelText = string.IsNullOrEmpty(displayName.stringValue)
			? label.text
			: displayName.stringValue;

		EditorGUI.PropertyField(position, highlighter, new GUIContent(labelText));
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight;
	}
}
}