using System.Collections;
using System.Collections.Generic;
using System.Text;
using EchoTerminal;
using UnityEngine;

namespace DefaultNamespace
{
public class ExampleMono : MonoBehaviour
{
	private static readonly string[] Messages =
	{
		"Player health is low",
		"Enemy spawned at origin",
		"Checkpoint reached",
		"Inventory full",
		"Connection timeout",
		"Asset bundle loaded",
		"Physics overlap detected",
		"Save file written",
		"Shader compiled",
		"Audio clip missing"
	};

	private Coroutine _spamCoroutine;

	[TerminalCommand("SpamLogs", "Toggle random log spam on or off")]
	private void SpamLogs(bool enabled)
	{
		if (enabled && _spamCoroutine == null)
		{
			_spamCoroutine = StartCoroutine(SpamRoutine());
			Debug.Log("Log spam started.");
		}
		else if (!enabled && _spamCoroutine != null)
		{
			StopCoroutine(_spamCoroutine);
			_spamCoroutine = null;
			Debug.Log("Log spam stopped.");
		}
		else
		{
			Debug.Log(enabled ? "Already running." : "Already stopped.");
		}
	}

	private IEnumerator SpamRoutine()
	{
		while (true)
		{
			var message = Messages[Random.Range(0, Messages.Length)];

			switch (Random.Range(0, 3))
			{
				case 0:
					Debug.Log(message);
					break;
				case 1:
					Debug.LogWarning(message);
					break;
				case 2:
					Debug.LogError(message);
					break;
			}

			yield return new WaitForSeconds(Random.Range(0.5f, 2f));
		}
	}

	[TerminalCommand]
	private void Teleport(float x, float y, float z)
	{
		transform.position = new(x, y, z);
		Debug.Log("New position: " + transform.position);
	}

	[TerminalCommand]
	private void Teleport(Vector3 position)
	{
		transform.position = new(position.x, position.y, position.z);
		Debug.Log("New position: " + transform.position);
	}

	[TerminalCommand]
	private void SetHealth(int value)
	{
		Debug.Log($"Health set to {value}");
	}

	[TerminalCommand]
	private void Say(string message)
	{
		Debug.Log($"[{gameObject.name}]: {message}");
	}

	[TerminalCommand]
	private void Toggle(bool enabled)
	{
		gameObject.SetActive(enabled);
		Debug.Log($"{gameObject.name} active: {enabled}");
	}

	[TerminalCommand]
	private void Move(Vector3 offset)
	{
		transform.position += offset;
		Debug.Log($"Moved to {transform.position}");
	}

	[TerminalCommand]
	private void Look(Vector2 direction)
	{
		Debug.Log($"Looking at ({direction.x}, {direction.y})");
	}

	[TerminalCommand]
	private void Paint(Color color)
	{
		var meshRenderer = GetComponentInChildren<MeshRenderer>();
		if (meshRenderer != null)
		{
			meshRenderer.sharedMaterial.color = color;
		}

		Debug.Log($"Color set to {color}");
	}

	[TerminalCommand]
	private void Speed(double value)
	{
		Debug.Log($"Speed set to {value:F4}");
	}

	[TerminalCommand]
	private void Damage(List<int> amounts)
	{
		var total = 0;
		foreach (var amount in amounts)
		{
			total += amount;
		}

		Debug.Log($"Dealt {amounts.Count} hits for {total} total damage");
	}

	[TerminalCommand]
	private void Waypoints(List<Vector3> points)
	{
		var sb = new StringBuilder();
		sb.Append($"Set {points.Count} waypoints:");
		for (var i = 0; i < points.Count; i++)
		{
			sb.Append($" [{i}]{points[i]}");
		}

		Debug.Log(sb.ToString());
	}

	[TerminalCommand]
	private void Path(List<Vector2> points)
	{
		var sb = new StringBuilder();
		sb.Append($"Set {points.Count} path points:");
		for (var i = 0; i < points.Count; i++)
		{
			sb.Append($" [{i}]{points[i]}");
		}

		Debug.Log(sb.ToString());
	}

	[TerminalCommand]
	private void Scales(List<float> values)
	{
		var sb = new StringBuilder();
		sb.Append($"{values.Count} scale values:");
		foreach (var v in values)
		{
			sb.Append($" {v:F2}");
		}

		Debug.Log(sb.ToString());
	}

	[TerminalCommand]
	private void Flags(List<bool> flags)
	{
		var sb = new StringBuilder();
		sb.Append($"{flags.Count} flags:");
		foreach (var f in flags)
		{
			sb.Append($" {f}");
		}

		Debug.Log(sb.ToString());
	}

	[TerminalCommand]
	private void Palette(List<Color> colors)
	{
		var sb = new StringBuilder();
		sb.Append($"{colors.Count} colors:");
		foreach (var c in colors)
		{
			sb.Append($" {c}");
		}

		Debug.Log(sb.ToString());
	}

	[TerminalCommand]
	private void Tags(List<string> tags)
	{
		var sb = new StringBuilder();
		sb.Append($"{tags.Count} tags:");
		foreach (var t in tags)
		{
			sb.Append($" \"{t}\"");
		}

		Debug.Log(sb.ToString());
	}
}
}