using UnityEngine;
using UnityEngine.UIElements;

namespace EchoTerminal
{
[CreateAssetMenu(fileName = "TerminalConfig", menuName = "Echo Terminal/UI Config")]
public class TerminalConfig : ScriptableObject
{
	[Header("Templates")]
	[SerializeField] private VisualTreeAsset _logEntryTemplate;

	[Header("Cursors")]
	[SerializeField] private TerminalCursorSet _cursorSet;

	[Header("Window Settings")]
	[SerializeField] private TerminalDragConstraints _dragConstraints;

	public VisualTreeAsset LogEntryTemplate => _logEntryTemplate;
	public TerminalCursorSet CursorSet => _cursorSet;
	public TerminalDragConstraints DragConstraints => _dragConstraints;
}
}
