using Cinemachine;
using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class PlayerFistCollider : MonoBehaviour
	{
		[SerializeField] private float punchImpulse, uppercutImpulse;
		private CinemachineImpulseSource _impulse;

		private PlayerState _state;
		private Transform _transform;
		
		private bool _inHitCooldown;

		private void Start()
		{
			_state = transform.root.GetComponent<PlayerState>();
			_impulse = GetComponent<CinemachineImpulseSource>();
			_transform = transform;
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
			
			var position = _transform.position;
			_impulse.GenerateImpulseAt(position, 
									   (other.transform.position - position).normalized 
									 * ( currentAttack == PlayerAttackType.LPunch ? punchImpulse : uppercutImpulse));
		}
	}
}