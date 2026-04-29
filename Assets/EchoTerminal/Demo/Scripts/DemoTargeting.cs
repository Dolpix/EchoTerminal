using UnityEngine;

namespace EchoTerminal.Demos
{
public class DemoEnemy : MonoBehaviour
{
	// -- PER-INSTANCE STATE
	// Each instance keeps its own values. Targeting lets terminal commands
	// address these independently rather than broadcasting to all enemies.

	[SerializeField] private float _maxHealth = 100f;
	private float _health;
	private bool _stunned;

	private void Start()
	{
		_health = _maxHealth;
	}
	
	[TerminalCommand]
	[TerminalDescription("Set this enemy's health")]
	[TerminalTarget]
	private void SetHealth([Suggest("0", "25", "50", "75", "100")] float amount)
	{
		_health = Mathf.Clamp(amount, 0f, _maxHealth);
		Debug.Log($"[{gameObject.name}] Health → {_health}/{_maxHealth}");
	}
	
	[TerminalCommand]
	[TerminalDescription("Stun this enemy for one second")]
	[TerminalTarget]
	private void Stun()
	{
		_stunned = true;
		Debug.Log($"[{gameObject.name}] Stunned!");
		Invoke(nameof(ClearStun), 1f);
	}

	private void ClearStun()
	{
		_stunned = false;
		Debug.Log($"[{gameObject.name}] Stun cleared.");
	}
	
	[TerminalCommand]
	[TerminalDescription("Move this enemy to a world position")]
	[TerminalTarget]
	private void MoveTo(Vector3 position)
	{
		transform.position = position;
		Debug.Log($"[{gameObject.name}] Moved to {transform.position}");
	}
	
	[TerminalCommand("Kill")]
	[TerminalDescription("Kill every enemy in the scene")]
	private void KillAll()
	{
		_health = 0f;
		Debug.Log($"[{gameObject.name}] Killed (broadcast).");
	}

	[TerminalCommand("Kill")]
	[TerminalDescription("Kill a specific enemy by target")]
	[TerminalTarget]
	private void KillTarget()
	{
		_health = 0f;
		Debug.Log($"[{gameObject.name}] Killed (targeted).");
	}
	
	[TerminalCommand]
	[TerminalDescription("Print this enemy's status to the log")]
	[TerminalTarget]
	private void PrintStatus([Inject] Terminal terminal)
	{
		Debug.Log(
			$"[{gameObject.name}] HP={_health}/{_maxHealth}  Stunned={_stunned}  " +
			$"(terminal has {terminal.Registry.GetCommandNames().Count} commands enabled)"
		);
	}
}
}