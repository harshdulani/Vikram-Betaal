using Betaal;
using Oldie;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class PlayerFistCollider : MonoBehaviour
	{
		[SerializeField] private float punchImpulse, uppercutImpulse;
		private CinemachineImpulseSource _impulse;

		private static PlayerState _state;
		private Transform _transform;
		
		private bool _inHitCooldown;

		private void Start()
		{
			if(!_state) _state = transform.root.GetComponent<PlayerState>();
			
			_impulse = GetComponent<CinemachineImpulseSource>();
			_transform = transform;
		}

		private void OnCollisionEnter(Collision other)
		{
			if(_inHitCooldown) return;
			
			PlayerAttackType currentAttack;
			if (other.collider.CompareTag("Betaal"))
			{
				if (!other.transform.root.TryGetComponent(out BetaalController betu)) return;

				_inHitCooldown = true;
				DOVirtual.DelayedCall(0.55f, () => _inHitCooldown = false);

				currentAttack = _state.CurrentAttackType;
				betu.GiveDamage(_state.Controller.GetAttackDamage(currentAttack), currentAttack);
			}
			
			else if (other.collider.CompareTag("Oldie"))
			{
				if (!other.transform.root.TryGetComponent(out OldieRefBank oldu)) return;

				_inHitCooldown = true;
				DOVirtual.DelayedCall(0.55f, () => _inHitCooldown = false);

				currentAttack = _state.CurrentAttackType;
				oldu.Combat.GetHit(_state.Controller.GetAttackDamage(currentAttack), currentAttack);
			}
			else return;
			
			var position = _transform.position;
			_impulse.GenerateImpulseAt(position,
									   (other.transform.position - position).normalized
									 * (currentAttack == PlayerAttackType.LPunch ? punchImpulse : uppercutImpulse));
		}
	}
}