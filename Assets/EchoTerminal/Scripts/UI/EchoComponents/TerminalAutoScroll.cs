using UnityEngine.UIElements;

namespace EchoTerminal.Components
{
public class TerminalAutoScroll : IEchoComponent
{
	private const float _bottomThreshold = 8f;

	private readonly ScrollView _scrollView;
	private readonly Terminal _terminal;
	private bool _scrollPending;

	public TerminalAutoScroll(Terminal terminal, VisualElement root)
	{
		_terminal = terminal;
		_scrollView = root?.Q<ScrollView>("log-scroll");

		if (_scrollView == null)
		{
			return;
		}

		_scrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(OnContentGeometryChanged);
		_terminal.OnSubmitted += OnSubmitted;
		_terminal.OnEntryAdded += OnEntryAdded;
	}

	~TerminalAutoScroll()
	{
		_terminal.OnSubmitted -= OnSubmitted;
		_terminal.OnEntryAdded -= OnEntryAdded;
	}

	private bool IsAtBottom()
	{
		var contentHeight = _scrollView.contentContainer.layout.height;
		var viewHeight = _scrollView.layout.height;
		return contentHeight - (_scrollView.scrollOffset.y + viewHeight) <= _bottomThreshold;
	}

	private void OnContentGeometryChanged(GeometryChangedEvent evt)
	{
		if (!_scrollPending)
		{
			return;
		}

		_scrollView.scrollOffset = new(0, float.MaxValue);
		_scrollPending = false;
	}

	private void OnSubmitted()
	{
		_scrollPending = true;
	}

	private void OnEntryAdded(TerminalEntry entry)
	{
		if (IsAtBottom())
		{
			_scrollPending = true;
		}
	}
}
}