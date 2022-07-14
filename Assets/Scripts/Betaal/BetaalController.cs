using DG.Tweening;
using UnityEngine;

namespace Betaal
{
	public class BetaalController : MonoBehaviour
	{
		[SerializeField] private HealthCanvas healthCanvas;
		[Range(0,1f), SerializeField] private float healthBeforeMidFightConv;
		[SerializeField] private int maxHealth;
		private int _currentHealth;

		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;

		[SerializeField] private GameObject[] lightningFx;

		[SerializeField] private Vector2 waitBetweenAttacksRange;
		public BetaalHandleAttack handleAttack;

		[HideInInspector] public BetaalBackArms arms;
		private Animator _anim;
		private Transform _transform;
		private Tween _fightingTween;
		private bool _hasHadMidFightConv;

		private static readonly int HitPunch = Animator.StringToHash("hitPunch");
		private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
		private static readonly int Dummy = Animator.StringToHash("dummy");

		public BetaalController() { handleAttack = new BetaalHandleAttack(this); }

		private void OnEnable()
		{
			GameEvents.BetaalFightStart += OnBetaalFightStart;
			GameEvents.ConversationStart += OnConversationStart;
		}

		private void OnDisable()
		{
			GameEvents.BetaalFightStart -= OnBetaalFightStart;
			GameEvents.ConversationStart -= OnConversationStart;
		}

		private void Start()
		{
			arms = GetComponent<BetaalBackArms>();
			_anim = GetComponent<Animator>();

			_transform = transform;
			_currentHealth = maxHealth;
			healthCanvas.DisableCanvas();
			
			HandleController.CalculateDuration(this);
		}

		private void Update() => Recenter();

		private void OnDrawGizmosSelected()
		{
			handleAttack.DrawGizmos();
		}

		private void StartCombat()
		{
			healthCanvas.EnableCanvas();
			_fightingTween = DOVirtual.DelayedCall(0.1f, ChooseAndLaunchAttack)
									  .SetRecyclable(true)
									  .SetAutoKill(false)
									  .OnStart(() => print("start"));
		}

		private void ChooseAndLaunchAttack()
		{
			var random = Random.value;

			if (random > 0.5f)
				StartHandleAttack();
			else
				StartArmsAttack();
		}

		private void StartArmsAttack()
		{
			arms.AttackChest();

			DOVirtual.DelayedCall(0.25f, () =>
										 {
											 var delay = Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y);
											 print(BetaalBackArms.AttackDuration + delay);
											 DOVirtual.DelayedCall(BetaalBackArms.AttackDuration + delay,
																   () => _fightingTween.Restart());
										 });
		}

		private void StartHandleAttack()
		{
			BetaalEvents.InvokeStartHandleAttack(this);
			BetaalEvents.InvokeStartBetaalAttack();

			DOVirtual.DelayedCall(0.25f, () =>
										 {
											 var delay = Random.Range(waitBetweenAttacksRange.x, waitBetweenAttacksRange.y);
											 print(HandleController.AttackDuration + delay);
											 DOVirtual.DelayedCall(HandleController.AttackDuration + delay,
																   () => _fightingTween.Restart());
										 });
		}

		private void EndCombat()
		{
			_fightingTween.Rewind();
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
				print("TODO: CODE TO START MIDFIGHT CONV");
			
			print(_currentHealth);
			if(_currentHealth > 0) return;
		
			_anim.enabled = false;
			GoRagdoll();
			EndCombat();
			healthCanvas.DisableCanvas();
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


		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);

		private void OnBetaalFightStart()
		{
			StartCombat();
			_hasHadMidFightConv = true;
		}

		private void OnConversationStart()
		{
			DOVirtual.DelayedCall(0.15f, () =>
										 {
											 if (GameManager.state.InConversationWithBetaal) EndCombat();
										 });
		}
	}
}