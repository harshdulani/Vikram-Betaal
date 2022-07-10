using System;
using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;

		[SerializeField] private float hitImpulseMultiplier;
		[SerializeField] private int lPunchDamage = 20, lUppercutDamage = 40;

		private PlayerState _my; 
		private Animator _anim;
		
		private static readonly int GetHurtBack = Animator.StringToHash("GetHurtBack");
		private static readonly int GetHurtFront = Animator.StringToHash("GetHurtFront");

		private void Awake()
		{
			DOTween.KillAll();
		}

		private void Start()
		{
			_anim = GetComponent<Animator>();
			_my = GetComponent<PlayerState>();
		}

		public int GetAttackDamage(PlayerAttackType type)
		{
			return type switch
				   {
					   PlayerAttackType.None => 0,
					   PlayerAttackType.LPunch => lPunchDamage,
					   PlayerAttackType.LUppercut => lUppercutDamage,
					   _ => throw new ArgumentOutOfRangeException(nameof (type), type, null)
				   };
		}

		public void GetHit(Vector3 damage)
		{
			_my.currentHealth -= damage.magnitude;

			if (_my.currentHealth < 0f)
			{
				GoRagdoll();
				return;
			}
			_my.Impulse.GenerateImpulse(damage.normalized * hitImpulseMultiplier);
			_anim.SetTrigger(Vector3.Dot(transform.forward, damage) > 0f ? GetHurtFront : GetHurtBack);
		}

		private void GoRagdoll()
		{
			_anim.enabled = false;
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-transform.forward * ragdollThrowBackForce, ForceMode.Impulse);
			}
		}
	}
}