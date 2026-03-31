using System;
using UnityEngine;

namespace EchoTerminal
{
[Serializable]
public struct TerminalEdgeConstraints
{
	[Min(0f)] 
	[Tooltip("Pixels from this screen edge that triggers snapping. 0 = disabled.")]
	public float SnapDistance;

	[Range(0f, 1f)]
	[Tooltip("How far the window can slide off this edge as a fraction of the window's size. 0 = fully bounded, 1 = can go fully off screen.")]
	public float Overhang;
}
}