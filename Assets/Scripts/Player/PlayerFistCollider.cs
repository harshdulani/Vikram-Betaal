using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class PlayerFistCollider : MonoBehaviour
	{
		private PlayerState _state;
		private bool _inHitCooldown;

		private void Start()
		{
			_state = transform.root.GetComponent<PlayerState>();
		}

		private void OnCollisionEnter(Collision other)
		{
			if(_inHitCooldown) return;
			if (!other.collider.CompareTag("Betaal")) return;

			if(!other.transform.root.TryGetComponent(out BetaalController betu)) return;

			_inHitCooldown = true;
			DOVirtual.DelayedCall(0.55f, () => _inHitCooldown = false);
			var currentAttack = _state.CurrentAttackType;
			betu.GiveDamage(_state.Controller.GetAttackDamage(currentAttack), currentAttack);
		}
	}
}