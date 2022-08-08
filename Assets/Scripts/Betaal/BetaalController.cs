using DG.Tweening;
using UnityEngine;

namespace Betaal
{
	public class BetaalController : MonoBehaviour
	{
		[SerializeField] private bool isRagdoll;
		[SerializeField] private int noOfDeaths;
		[SerializeField] private HealthCanvas healthCanvas;
		[Range(0,1f), SerializeField] private float healthBeforeMidFightConv;
		[SerializeField] private int maxHealth;
		private int _currentHealth;

		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;

		[SerializeField] private GameObject[] lightningFx;

		[SerializeField] private Vector2 waitBetweenAttacksRange;
		[SerializeField] private float minDistanceForHandleAttack;
		public BetaalHandleAttack handleAttack;

		[HideInInspector] public BetaalBackArms arms;
		[SerializeField] private GameObject pickup;
		private BetaalMovement _movement;
		private Animator _anim;
		private Transform _transform, _player;
		private int _currentDeathCount;
		private bool _hasHadMidFightConv, _isArmAttacking, _isHandleAttacking;

		private static readonly int HitPunch = Animator.StringToHash("hitPunch");
		private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
		private static readonly int Dummy = Animator.StringToHash("dummy");

		public BetaalController() { handleAttack = new BetaalHandleAttack(this); }

		private void OnEnable()
		{
			GameEvents.BetaalFightStart += OnBetaalFightStart;
			GameEvents.BetaalFightEnd += OnBetaalFightEnd;
			BetaalEvents.EndBetaalArmsAttack += OnArmsAttackEnd;
			BetaalEvents.EndBetaalHandleAttack += OnHandleAttackEnd;
			
			GameEvents.ConversationStart += OnConversationStart;
			
			GameEvents.GameLose += OnGameLose;
		}

		private void OnDisable()
		{
			GameEvents.BetaalFightStart -= OnBetaalFightStart;
			GameEvents.BetaalFightEnd -= OnBetaalFightEnd;
			BetaalEvents.EndBetaalArmsAttack -= OnArmsAttackEnd;
			BetaalEvents.EndBetaalHandleAttack -= OnBetaalFightStart;
			
			GameEvents.ConversationStart -= OnConversationStart;
			
			GameEvents.GameLose -= OnGameLose;
		}

	#if UNITY_EDITOR

		private void OnValidate()
		{
			if(!Application.isPlaying || !_anim) return;
			
			if (isRagdoll && _anim.enabled)
				GoRagdoll();
			else
				ResetRagdoll();
		}

	#endif
		
		private void Start()
		{
			_movement = GetComponent<BetaalMovement>();
			arms = GetComponent<BetaalBackArms>();
			_anim = GetComponent<Animator>();

			CheckAndInitPlayer();
			_transform = transform;
			_currentHealth = maxHealth;
			healthCanvas.DisableCanvas();

			HandleController.CalculateDuration(this);
		}

		private void OnDrawGizmosSelected()
		{
			handleAttack.DrawGizmos();
		}

		public void GiveDamage(int getAttackDamage, PlayerAttackType type)
		{
			_currentHealth -= getAttackDamage;
			_anim.SetTrigger(type switch
							 {
								 PlayerAttackType.LPunch => HitPunch,
								 PlayerAttackType.LUppercut => HitUppercut,
								 _ => Dummy
							 });

			var healthNormalised = _currentHealth / (float) maxHealth;
			healthCanvas.SetHealth(healthNormalised);
			
			if(healthNormalised < healthBeforeMidFightConv && !_hasHadMidFightConv) 
				StartMidFightCon();

			if(_currentHealth > 0) return;
		
			Die();
		}

		public void DeRagdoll()
		{
			_anim.enabled = true;
			
			foreach (var fx in lightningFx) fx.SetActive(true);
			
			foreach (var rb in rigidbodies) rb.isKinematic = true;
		}

		private void StartMidFightCon()
		{
			_hasHadMidFightConv = true;
			GameManager.state.startFightAfterNextConversation = true;

			GameEvents.InvokeConversationStart();
			GameEvents.InvokeBetaalFightEnd(true);
			
			Lightning.only.CustomLightning(3500000);
			DOVirtual.DelayedCall(0.15f, () =>
										 {
											 _transform.position = _movement.initPos;
											 _player.position = _movement.initPos += Vector3.left * _movement.distanceFromPlayerRange.x;
										 });
		}


		private void StartCombat()
		{
			healthCanvas.EnableCanvas();
			
			_isHandleAttacking = false;
			_isArmAttacking = false;
			
			ChooseAndLaunchAttack();
		}

		private void ChooseAndLaunchAttack()
		{
			//if(_isArmAttacking || _isHandleAttacking) return;
			CheckAndInitPlayer();
			if (Vector3.Distance(_transform.position, _player.position) > minDistanceForHandleAttack)
				StartHandleAttack();
			else
				StartArmsAttack();
		}

		public void StartArmsAttack()
		{
			if(_isArmAttacking) return;
			
			_isArmAttacking = true;
			arms.AttackChest();
		}

		private void StartHandleAttack()
		{
			if(_isHandleAttacking) return;

			_isHandleAttacking = true;
			BetaalEvents.InvokeStartHandleAttack(this);
		}

		private void EndCombat()
		{
			DOTween.Kill(arms);
			DOTween.Kill(this);
		}

		private void Die()
		{
			if (_currentDeathCount++ <= noOfDeaths) GameEvents.InvokeConversationStart();
			
			_anim.enabled = false;
			GoRagdoll();
			EndCombat();
			healthCanvas.DisableCanvas();
			GameEvents.InvokeBetaalFightEnd();
			_movement.StopMovementTween();
			_hasHadMidFightConv = false;
			pickup.SetActive(true);
			
			if (!GameManager.state.betaalFight1Over)
			{
				GameManager.state.betaalFight1Over = true;
				return;
			}

			if (!GameManager.state.betaalFight2Over)
			{
				GameManager.state.betaalFight2Over = true;
			}
		}

		private void GoRagdoll()
		{
			_anim.enabled = false;
			
			foreach (var fx in lightningFx) fx.SetActive(false);
			
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-_transform.forward * ragdollThrowBackForce + Vector3.up, ForceMode.Impulse);
			}
		}

		private void ResetRagdoll()
		{
			_anim.enabled = true;
			
			foreach (var fx in lightningFx) fx.SetActive(true);
			
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = true;
				rb.ResetInertiaTensor();
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
			}
		}

		private void CheckAndInitPlayer()
		{
			if(!_player)
				_player = GameObject.FindGameObjectWithTag("Player").transform.root;
		}

		private void OnBetaalFightStart()
		{
			StartCombat();
			_movement.StartMovementTween();
			_movement.AssignNewHomePos();
		}

		private void OnArmsAttackEnd()
		{
			_isArmAttacking = false;
			DOVirtual.DelayedCall(Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y), ChooseAndLaunchAttack);
		}

		private void OnHandleAttackEnd()
		{
			_isHandleAttacking = false;
			DOVirtual.DelayedCall(Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y), ChooseAndLaunchAttack);
		}

		private void OnConversationStart()
		{
			EndCombat();
			_movement.StopMovementTween();

			DOVirtual.DelayedCall(0.15f, () =>
										 {
											 if (!GameManager.state.InConversationWithBetaal) return;
											 
											 _movement.StopMovementTween();
											 EndCombat();
										 });
		}

		private void OnBetaalFightEnd(bool isTemporary)
		{
			if (!isTemporary)
			{
				_currentHealth = maxHealth;
				var healthNormalised = _currentHealth / (float) maxHealth;
				healthCanvas.SetHealth(healthNormalised);
			}

			EndCombat();
		}

		private void OnGameLose()
		{
			EndCombat();
			_movement.StopMovementTween();
		}
	}
}