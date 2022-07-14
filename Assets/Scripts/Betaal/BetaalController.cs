using DG.Tweening;
using UnityEngine;

namespace Betaal
{
	public class BetaalController : MonoBehaviour
	{
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
		private BetaalMovement _movement;
		private Animator _anim;
		private Transform _transform, _player;
		private int _currentDeathCount;
		private bool _hasHadMidFightConv;

		private static readonly int HitPunch = Animator.StringToHash("hitPunch");
		private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
		private static readonly int Dummy = Animator.StringToHash("dummy");
		private bool _isDead, _isArmAttacking, _isHandleAttacking;

		public BetaalController() { handleAttack = new BetaalHandleAttack(this); }

		private void OnEnable()
		{
			GameEvents.BetaalFightStart += OnBetaalFightStart;
			GameEvents.ConversationStart += OnConversationStart;
			
			GameEvents.GameLose += OnGameLose;
		}

		private void OnDisable()
		{
			GameEvents.BetaalFightStart -= OnBetaalFightStart;
			GameEvents.ConversationStart -= OnConversationStart;
			
			GameEvents.GameLose -= OnGameLose;
		}

		private void Start()
		{
			_movement = GetComponent<BetaalMovement>();
			arms = GetComponent<BetaalBackArms>();
			_anim = GetComponent<Animator>();

			_player = GameObject.FindGameObjectWithTag("Player").transform.root;
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
			if(_isDead) return;
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

		private void StartMidFightCon()
		{
			_hasHadMidFightConv = true;
			GameEvents.InvokeConversationStart();
			GameEvents.InvokeBetaalFightEnd(true);
			
			Lightning.only.CustomLightning(3500000);
			DOVirtual.DelayedCall(0.15f, () =>
										 {
											 _transform.position = _movement.initPos;
											 _player.position = _movement.initPos += Vector3.left * _movement.maxDistanceFromPlayer;
										 });
		}
		

		private void StartCombat()
		{
			healthCanvas.EnableCanvas();
			ChooseAndLaunchAttack();
		}

		private void ChooseAndLaunchAttack()
		{
			_isArmAttacking = false;
			_isHandleAttacking = false;

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

			DOVirtual.DelayedCall(0.25f, () =>
										 {
											 var delay = Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y);
											 DOVirtual.DelayedCall(BetaalBackArms.AttackDuration + delay, ChooseAndLaunchAttack)
													  .SetTarget(this);
										 })
					 .SetTarget(this);
		}

		private void StartHandleAttack()
		{
			if(_isHandleAttacking) return;

			_isHandleAttacking = true;
			
			BetaalEvents.InvokeStartHandleAttack(this);
			BetaalEvents.InvokeStartBetaalAttack();

			DOVirtual.DelayedCall(0.25f, () =>
										 {
											 var delay = Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y);
											 DOVirtual.DelayedCall(HandleController.AttackDuration + delay,
																   ChooseAndLaunchAttack).SetTarget(this);
										 })
					 .SetTarget(this);
		}

		private void EndCombat()
		{
			DOTween.Kill(arms);
			DOTween.Kill(this);
		}

		private void Die()
		{
			_isDead = true;
			if (_currentDeathCount++ <= noOfDeaths) GameEvents.InvokeConversationStart();
			
			_anim.enabled = false;
			GoRagdoll();
			EndCombat();
			healthCanvas.DisableCanvas();
			GameEvents.InvokeBetaalFightEnd();
			_movement.StopMovementTween();
			_hasHadMidFightConv = false;
		}

		private void GoRagdoll()
		{
			_anim.enabled = false;
			
			foreach (var fx in lightningFx) fx.SetActive(false);
			
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-_transform.forward * ragdollThrowBackForce, ForceMode.Impulse);
			}
		}

		private void OnBetaalFightStart()
		{
			StartCombat();
			_movement.StartMovementTween();
			_movement.AssignNewHomePos();
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

		private void OnGameLose()
		{
			EndCombat();
			_movement.StopMovementTween();
		}
	}
}