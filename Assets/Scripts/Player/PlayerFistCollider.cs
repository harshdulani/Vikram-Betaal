using Player.Combat;
using UnityEngine;

namespace Player
{
	public class PlayerFistCollider : MonoBehaviour
	{
		private PlayerState _state;

		private void Start()
		{
			_state = transform.root.GetComponent<PlayerState>();
		}

		private void OnCollisionEnter(Collision other)
		{
			print(other.transform);
			if (!other.collider.CompareTag("Betaal")) return;
			print(1);
			
			if(!other.transform.root.TryGetComponent(out BetaalController betu)) return;
			print(1);

			var currentAttack = _state.CurrentAttackType;
			betu.GiveDamage(_state.Controller.GetAttackDamage(currentAttack), currentAttack);
		}
	}
}