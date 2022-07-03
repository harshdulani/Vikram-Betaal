using Player;
using UnityEngine;

public class OldieProjectileAttack : MonoBehaviour
{
	[SerializeField] private int damage;
	private bool _hasAttacked;

	private void OnTriggerEnter(Collider other)
	{
		if(_hasAttacked) return;
		if(!other.CompareTag("Player")) return;
		if(!other.transform.root.TryGetComponent(out PlayerController player)) return;

		player.GetHit((other.transform.position - transform.position).normalized * damage);
		_hasAttacked = true;
	}
}
