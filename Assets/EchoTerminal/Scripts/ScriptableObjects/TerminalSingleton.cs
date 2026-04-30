#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal.Scripts.ScriptableObjects
{
[CreateAssetMenu(fileName = "TerminalSingleton", menuName = "Echo Terminal/Assets")]
public class TerminalSingleton : ScriptableObject
{
	private static TerminalSingleton _instance;
	[SerializeField] private VisualTreeAsset _uxml;
	[SerializeField] private StyleSheet _styleSheet;
	[SerializeField] private TerminalConfig _config;

	public VisualTreeAsset Uxml => _uxml;
	public StyleSheet StyleSheet => _styleSheet;
	public TerminalConfig Config => _config;

	public static TerminalSingleton Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}

			string[] guids = AssetDatabase.FindAssets("t:TerminalSingleton");
			if (guids.Length == 0)
			{
				return null;
			}

			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			_instance = AssetDatabase.LoadAssetAtPath<TerminalSingleton>(path);
			return _instance;
		}
	}
}
}
#endif