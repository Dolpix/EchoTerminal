using UnityEngine;

namespace EchoTerminal.Scripts.ScriptableObjects
{
[CreateAssetMenu(fileName = "TerminalCursorSet", menuName = "Echo Terminal/Cursor Set")]
public class TerminalCursorSet : ScriptableObject
{
	[Header("Cursors")]
	[SerializeField] private Texture2D _move;
	[SerializeField] private Texture2D _resizeEw;
	[SerializeField] private Texture2D _resizeNs;
	[SerializeField] private Texture2D _resizeNwse;
	[SerializeField] private Texture2D _resizeNesw;

	[Header("Hotspot")]
	[Tooltip("Pixel offset from the top-left of the cursor texture to its active point. For a centered 64x64 cursor use (32, 32).")]
	[SerializeField]
	private Vector2 _hotspot = new(32f, 32f);

	public Texture2D Move => _move;
	public Texture2D ResizeEw => _resizeEw;
	public Texture2D ResizeNs => _resizeNs;
	public Texture2D ResizeNwse => _resizeNwse;
	public Texture2D ResizeNesw => _resizeNesw;
	public Vector2 Hotspot => _hotspot;
}
}