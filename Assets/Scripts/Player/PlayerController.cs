using System;
using DG.Tweening;
using Player.Combat;
using UnityEngine;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private HealthCanvas healthCanvas;
		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;

		[SerializeField] private float hitImpulseMultiplier;
		[SerializeField] private int lPunchDamage = 20, lUppercutDamage = 40;

		public bool allowedInteractionWithBetaalChange = true;
		public bool canInteractWithBetaal;

		private PlayerState _my;
		private Animator _anim;

		private static readonly int GetHurtBack = Animator.StringToHash("GetHurtBack");
		private static readonly int GetHurtFront = Animator.StringToHash("GetHurtFront");

		private void OnEnable()
		{
			PlayerInput.UsePressed += OnUsePressed;

			GameEvents.ConversationStart += OnConversationStart;
			GameEvents.BetaalFightStart += OnFightEnter;
			GameEvents.BetaalFightEnd += OnBetaalFightEnd;
		}

		private void OnDisable()
		{
			PlayerInput.UsePressed -= OnUsePressed;
			
			GameEvents.ConversationStart -= OnConversationStart;

			GameEvents.BetaalFightStart -= OnFightEnter;
			GameEvents.BetaalFightEnd -= OnBetaalFightEnd;
		}

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

			var normalisedHealth = _my.currentHealth / _my.maxHealth;
			healthCanvas.SetHealth(normalisedHealth);
			
			if (_my.currentHealth < 0f)
			{
				Die();
				return;
			}
			_my.Impulse.GenerateImpulse(damage.normalized * hitImpulseMultiplier);
			_anim.SetTrigger(Vector3.Dot(transform.forward, damage) > 0f ? GetHurtFront : GetHurtBack);
		}

		public bool TryInteractWithBetaalStatusChange(bool b)
		{
			if(!allowedInteractionWithBetaalChange) return false;

			canInteractWithBetaal = b;
			return true;
		}


		public void GetPushedBack() => transform.position -= Vector3.right;

		private void Die()
		{
			GoRagdoll();
			healthCanvas.DisableCanvas();
			GameEvents.InvokeGameLose();
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

		private void OnUsePressed()
		{
			if (!canInteractWithBetaal) return;

			canInteractWithBetaal = false;
			allowedInteractionWithBetaalChange = false;
			GameEvents.InvokeInteractWithBetaal();
		}

		private void OnFightEnter()
		{
			healthCanvas.EnableCanvas();
		}
		
		private void OnBetaalFightEnd(bool isTemporary)
		{
			healthCanvas.DisableCanvas();
			if(!isTemporary)
				_my.currentHealth = _my.maxHealth;
		}

		private void OnConversationStart()
		{
			healthCanvas.DisableCanvas();
		}
	}
}