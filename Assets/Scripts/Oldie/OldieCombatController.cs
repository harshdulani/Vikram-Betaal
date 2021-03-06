using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Oldie
{
	public class OldieCombatController : MonoBehaviour
	{
		public bool isInCombat;

		[SerializeField] private float combatBlendValue = 0.5f;
		[SerializeField] private float maxDistanceFromPlayer, aiAttackInterval, aiRepositionInterval;
		[SerializeField] private float proximityCooldownDuration;

		[SerializeField] private ProjectileAttack projectile;
		[SerializeField] private ProximityAttack proximity;

		[SerializeField] private List<Rigidbody> rigidbodies;
		[SerializeField] private int maxHealth;
		private int _currentHealth;

		private Transform _player, _transform;
		private bool _inProximityAttackCooldown;

		private OldieRefBank _my;

		private Tween _movementTween, _attackTween;

		private static readonly int MovementCombatBlendValue = Animator.StringToHash("movementCombatBlendValue");
		private static readonly int HitPunch = Animator.StringToHash("hitPunch");
		private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
		private static readonly int Dummy = Animator.StringToHash("dummy");

		private void Start()
		{
			_my = GetComponent<OldieRefBank>();

			_player = GameObject.FindGameObjectWithTag("Player").transform.root;
			_transform = transform;

			_currentHealth = maxHealth;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.U)) StartAttackAI();
		}

		private void OnValidate()
		{
			if(!Application.isPlaying) return;

			if(isInCombat)
				StartAttackAI();
		}

		private void StartAttackAI()
		{
			SetCombatBlendValue();

			isInCombat = true;
			_attackTween = DOVirtual.DelayedCall(aiAttackInterval, 
												 () =>
												 {
													 if (Random.value > 0.5f) projectile.StartAttack(_player);
												 }).SetLoops(-1);

			_movementTween = DOVirtual.DelayedCall(aiRepositionInterval,
												   () =>
												   {
													   var vector = _transform.position - _player.position;
													   print(vector);
													   if (Vector3.Distance(_player.position, _transform.position) 
														 > maxDistanceFromPlayer)
														   FindNewPosition(vector);
												   }).SetLoops(-1);
		}

		public void GetHit(int getAttackDamage, PlayerAttackType type)
		{
			_currentHealth -= getAttackDamage;
			_my.Animator.SetTrigger(type switch
							 {
								 PlayerAttackType.LPunch => HitPunch,
								 PlayerAttackType.LUppercut => HitUppercut,
								 _ => Dummy
							 });
		
			print(_currentHealth);
			if(_currentHealth > 0) return;
		
		}

		public void Die()
		{
			GoRagdoll();
			_my.IsDead = false;
			_attackTween.Kill();
			_movementTween.Kill();
			
			GameEvents.InvokeGameWin();
		}
		
		private void GoRagdoll()
		{
			_my.Animator.enabled = false;

			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-transform.forward * 2f, ForceMode.Impulse);
			}
		}

		private void FindNewPosition(Vector3 vector)
		{
			print("find new pos");
			var dest = _player.position + vector.normalized * maxDistanceFromPlayer;
			_my.Movement.SetNewDest(dest);
		}

		private void LaunchProximityReductionAttack()
		{
			proximity.StartAttack(_player);
		
			_inProximityAttackCooldown = true;
			DOVirtual.DelayedCall(proximityCooldownDuration, () => _inProximityAttackCooldown = false);
		}
	
		private void SetCombatBlendValue() => DOTween.To(BlendValueGetter, BlendValueSetter, combatBlendValue, 0.5f);
		private float BlendValueGetter() => _my.Animator.GetFloat(MovementCombatBlendValue);
		private void BlendValueSetter(float value) => _my.Animator.SetFloat(MovementCombatBlendValue, value);

		private void OnTriggerEnter(Collider other)
		{
			if (!isInCombat) return;
			if(_inProximityAttackCooldown) return;
			if(!other.CompareTag("Player")) return;

			LaunchProximityReductionAttack();
		}
	}
}
