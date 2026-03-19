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
	private string SpamLogs(bool enabled)
	{
		if (enabled && _spamCoroutine == null)
		{
			_spamCoroutine = StartCoroutine(SpamRoutine());
			return "Log spam started.";
		}

		if (!enabled && _spamCoroutine != null)
		{
			StopCoroutine(_spamCoroutine);
			_spamCoroutine = null;
			return "Log spam stopped.";
		}

		return enabled ? "Already running." : "Already stopped.";
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
	private string SetHealth(int value)
	{
		return $"Health set to {value}";
	}

	[TerminalCommand]
	private string Say(string message)
	{
		return $"[{gameObject.name}]: {message}";
	}

	[TerminalCommand]
	private string Toggle(bool enabled)
	{
		gameObject.SetActive(enabled);
		return $"{gameObject.name} active: {enabled}";
	}

	[TerminalCommand]
	private string Move(Vector3 offset)
	{
		transform.position += offset;
		return $"Moved to {transform.position}";
	}

	[TerminalCommand]
	private string Look(Vector2 direction)
	{
		return $"Looking at ({direction.x}, {direction.y})";
	}

	[TerminalCommand]
	private string Paint(Color color)
	{
		var meshRenderer = GetComponentInChildren<MeshRenderer>();

		if (meshRenderer != null)
		{
			meshRenderer.sharedMaterial.color = color;
		}

		return $"Color set to {color}";
	}

	[TerminalCommand]
	private string Speed(double value)
	{
		return $"Speed set to {value:F4}";
	}

	[TerminalCommand]
	private string Damage(List<int> amounts)
	{
		var total = 0;

		foreach (var amount in amounts)
		{
			total += amount;
		}

		return $"Dealt {amounts.Count} hits for {total} total damage";
	}

	[TerminalCommand]
	private string Waypoints(List<Vector3> points)
	{
		var sb = new StringBuilder();
		sb.Append($"Set {points.Count} waypoints:");

		for (var i = 0; i < points.Count; i++)
		{
			sb.Append($" [{i}]{points[i]}");
		}

		return sb.ToString();
	}

	[TerminalCommand]
	private string Path(List<Vector2> points)
	{
		var sb = new StringBuilder();
		sb.Append($"Set {points.Count} path points:");

		for (var i = 0; i < points.Count; i++)
		{
			sb.Append($" [{i}]{points[i]}");
		}

		return sb.ToString();
	}

	[TerminalCommand]
	private string Scales(List<float> values)
	{
		var sb = new StringBuilder();
		sb.Append($"{values.Count} scale values:");

		foreach (var v in values)
		{
			sb.Append($" {v:F2}");
		}

		return sb.ToString();
	}

	[TerminalCommand]
	private string Flags(List<bool> flags)
	{
		var sb = new StringBuilder();
		sb.Append($"{flags.Count} flags:");

		foreach (var f in flags)
		{
			sb.Append($" {f}");
		}

		return sb.ToString();
	}

	[TerminalCommand]
	private string Palette(List<Color> colors)
	{
		var sb = new StringBuilder();
		sb.Append($"{colors.Count} colors:");

		foreach (var c in colors)
		{
			sb.Append($" {c}");
		}

		return sb.ToString();
	}

	[TerminalCommand]
	private string Tags(List<string> tags)
	{
		var sb = new StringBuilder();
		sb.Append($"{tags.Count} tags:");

		foreach (var t in tags)
		{
			sb.Append($" \"{t}\"");
		}

		return sb.ToString();
	}
}
}