using DG.Tweening;
using Player;
using Player.Combat;
using UnityEngine;

namespace Betaal
{
	public class HandleController : MonoBehaviour
	{
		public static float AttackDuration = 0f;
		
		private Vector3 _initPos;
		private bool _isAttacking, _hasAttacked;

		private static PlayerInput _player;
		private Rigidbody _rb;
		private Transform _transform;

		private void OnEnable()
		{
			BetaalEvents.StartHandleAttack += OnStartHandleAttack;
		}
	
		private void OnDisable()
		{
			BetaalEvents.StartHandleAttack -= OnStartHandleAttack;
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			if(!_player)
				_player = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerInput>();
			
			_transform = transform;
			_initPos = _transform.position;
		}

		public void TryRotate(Vector3 torqueMultiplier, ForceMode velocityChange)
		{
			if(_isAttacking) return;
			_rb.AddTorque(torqueMultiplier, velocityChange);
		}

		private void StopAttacking()
		{
			_isAttacking = false; 
			_hasAttacked = false;
		}

		public static void CalculateDuration(BetaalController betaal)
		{
			if(AttackDuration > 0f) return;
			
			AttackDuration = 0f;
			AttackDuration += betaal.handleAttack.shakeTime;
			AttackDuration += 0.5f;
			AttackDuration += betaal.handleAttack.postAttackInterval;
			AttackDuration += 1.25f;
			AttackDuration += 0.1f;
		}

		private void OnStartHandleAttack(BetaalController betaal)
		{
			if(Vector3.Distance(betaal.transform.position, _transform.position) > betaal.handleAttack.maxDistanceForAttack)
				return;

			_isAttacking = true;
			_rb.isKinematic = true;

			var myAttackerIndex = betaal.handleAttack.IncreaseHandleCount(_transform);
		
			var seq = DOTween.Sequence();
			
			//add interval for each handle so they attack like a missile drop
			seq.AppendInterval(myAttackerIndex * betaal.handleAttack.perAttackInterval);
			
			//Shake rotation of handle to make it look like its popping out
			seq.Append(_transform.DOShakeRotation(
												  betaal.handleAttack.shakeTime, 
												  betaal.handleAttack.shakeStrength, 
												  10, 90f, false));
			seq.Join(_transform.DOMoveY(_initPos.y - 0.5f, betaal.handleAttack.shakeTime));
			
			//store start pos to calculate direction to add force in when dotween to target gets over
			//since target is in the air
			var startPos = Vector3.zero;

			seq.AppendCallback(() =>
							   {
								   _rb.isKinematic = false;
								   startPos = _transform.position;
								   betaal.handleAttack.EndHandleAttackControl(_transform);
							   });
			
			betaal.handleAttack.attackPos = _player.attackHostPosition.position;
			//TODO: add electric explosion here as he shoots the handle at vikram
			//move this handle to the attack position decided by betaal, and make sure it keeps flying towards goal
			seq.Append(_transform.DOMove(betaal.handleAttack.attackPos, 0.5f).SetEase(Ease.InExpo));
			seq.AppendCallback(() => _rb.AddForce((_transform.position - startPos).normalized * 15f, ForceMode.Impulse));

			//stay on the floor for some time
			seq.AppendInterval(betaal.handleAttack.postAttackInterval);
			
			seq.AppendCallback(() => _rb.isKinematic = true);

			//fly back to the position they were at, on game start
			seq.Join(_transform.DOMoveX(_initPos.x, 1f));
			seq.Join(_transform.DOMoveY(_initPos.y, 1.25f).SetEase(Ease.OutExpo));
			seq.Join(_transform.DOMoveZ(_initPos.z, 1f));
			seq.Join(_transform.DORotate(Vector3.zero, 1f));
			
			seq.AppendInterval(0.1f);
			
			seq.AppendCallback(() =>
							   {
								   //let this handle be rotated to simulate train rumble again
								   _rb.isKinematic = false;
								   StopAttacking();
								   BetaalEvents.InvokeEndBetaalHandleAttack();
							   });
		}

		private void OnCollisionEnter(Collision other)
		{
			if(!_isAttacking) return;
			if(_hasAttacked) return;
			if(!other.transform.root.CompareTag("Player")) return;

			var player = other.transform.root.GetComponent<PlayerState>();

			if(!player.CanGetAttacked()) return;
			
			_hasAttacked = true;
			player.Controller.GetHit(other.relativeVelocity);
		}
	}
}