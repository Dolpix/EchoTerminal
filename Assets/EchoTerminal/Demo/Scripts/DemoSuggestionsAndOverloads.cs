using System.Collections.Generic;
using EchoTerminal.Scripts.TerminalCore.Attributes;
using UnityEngine;

namespace EchoTerminal.Demo.Scripts
{
public static class DemoSuggestionsAndOverloads
{
	// -- LITERAL SUGGESTIONS
	// [Suggest("a", "b", "c")] tells the terminal to offer those strings as
	// completions when the cursor is on that parameter. The user can still type
	// anything - suggestions are hints, not hard constraints.
	//
	//   Type:  SetVolume 0.5      (or tab-complete from 0 / 0.5 / 1)
	[TerminalCommand]
	[TerminalDescription("Set the global audio volume (0–1)")]
	private static void SetVolume([Suggest("0", "0.25", "0.5", "0.75", "1")] float volume)
	{
		AudioListener.volume = Mathf.Clamp01(volume);
		Debug.Log($"Volume → {AudioListener.volume:F2}");
	}
	

	// -- OVERLOADS
	// You can register multiple methods under the same command name by giving
	// them the same [TerminalCommand] name (or letting the method name be used
	// for both). The terminal picks the overload whose parameter types best
	// match what the user typed.
	//
	// The parser tries every overload and scores them. The first fully-matched
	// overload wins. Partial matches are ranked by how many tokens completed.

	[TerminalCommand("Resize")]
	[TerminalDescription("Set the render texture width and height")]
	private static void ResizeByValues(int width, int height)
	{
		Debug.Log($"Resize → {width} × {height}  (from two ints)");
	}

	[TerminalCommand("Resize")]
	[TerminalDescription("Set the render texture size from a Vector2")]
	private static void ResizeByVector(Vector2 size)
	{
		Debug.Log($"Resize → {(int)size.x} × {(int)size.y}  (from Vector2)");
	}

	// Three overloads: zero args resets, one arg sets both axes, two args set each.
	//   Type:  SetFog                 → disables fog
	//   Type:  SetFog 0.02            → uniform density
	//   Type:  SetFog 0.01 0.05       → start and end density

	[TerminalCommand("SetFog")]
	[TerminalDescription("Disable fog")]
	private static void SetFogOff()
	{
		RenderSettings.fog = false;
		Debug.Log("Fog disabled.");
	}

	[TerminalCommand("SetFog")]
	[TerminalDescription("Enable fog with a uniform density")]
	private static void SetFogUniform(float density)
	{
		RenderSettings.fog = true;
		RenderSettings.fogDensity = density;
		Debug.Log($"Fog on — density {density}");
	}

	[TerminalCommand("SetFog")]
	[TerminalDescription("Enable fog with separate start and end distances")]
	private static void SetFogRange(float startDist, float endDist)
	{
		RenderSettings.fog = true;
		RenderSettings.fogMode = FogMode.Linear;
		RenderSettings.fogStartDistance = startDist;
		RenderSettings.fogEndDistance = endDist;
		Debug.Log($"Fog on — linear [{startDist}..{endDist}]");
	}

	// -- LIST PARAMETERS
	// List<T> lets the user pass multiple values of the same type as a bracketed
	// list. Any supported type works as the element type.
	//
	//   Type:  SumAll [1, 2, 3, 4, 5]    → logs  Sum: 15
	//   Type:  PrintTags ["speed", "cheat", "debug"]
	[TerminalCommand]
	[TerminalDescription("Log the sum of a list of floats")]
	private static void SumAll(List<float> values)
	{
		var total = 0f;
		foreach (float v in values)
		{
			total += v;
		}

		Debug.Log($"Sum of {values.Count} value(s): {total}");
	}
}
}