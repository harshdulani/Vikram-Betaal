using Player.Combat;
using UnityEngine;

namespace Betaal
{
	public class BetaalController : MonoBehaviour
	{
		[SerializeField] private int maxHealth;
		private int _currentHealth;

		[SerializeField] private Rigidbody[] rigidbodies;
		[SerializeField] private float ragdollThrowBackForce;

		[SerializeField] private GameObject lightningFx;

		private Animator _anim;
		private Transform _transform;

		public BetaalHandleAttack handleAttack;
		private PlayerCombat _player;

		private static readonly int HitPunch = Animator.StringToHash("hitPunch");
		private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
		private static readonly int Dummy = Animator.StringToHash("dummy");
	
		public BetaalController() { handleAttack = new BetaalHandleAttack(this); }

		private void Start()
		{
			_anim = GetComponent<Animator>();
			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();

			_transform = transform;
			_currentHealth = maxHealth;
		}

		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.K)) GoRagdoll();
			if (Input.GetKeyDown(KeyCode.P)) StartHandleAttack();
			Recenter();
		}

		private void OnDrawGizmos()
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
		
			print(_currentHealth);
			if(_currentHealth > 0) return;
		
			_anim.enabled = false;
			GoRagdoll();
		}

		private void StartHandleAttack()
		{
			handleAttack.attackPos = _player.attackHostPosition.position;
			BetaalEvents.InvokeStartHandleAttack(this);
		}

		private void GoRagdoll()
		{
			_anim.enabled = false;
			lightningFx.SetActive(false);
			foreach (var rb in rigidbodies)
			{
				rb.isKinematic = false;
				rb.AddForce(-_transform.forward * ragdollThrowBackForce, ForceMode.Impulse);
			}
		}

		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);
	}
}