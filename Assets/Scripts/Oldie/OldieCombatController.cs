using DG.Tweening;
using UnityEngine;

public class OldieCombatController : MonoBehaviour
{
	public bool isInCombat;

	[SerializeField] private float maxDistanceFromPlayer, aiDecisionInterval;
	[SerializeField] private float proximityCooldownDuration;

	[SerializeField] private ProjectileAttack projectile;
	[SerializeField] private ProximityAttack proximity;

	private Transform _player, _transform;
	private bool _inProximityAttackCooldown;

	private Animator _anim;
	private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

	private void Start()
	{
		_anim = GetComponent<Animator>();
		
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_transform = transform;
	}

	private void OnValidate()
	{
		if(!Application.isPlaying) return;

		if(isInCombat)
			StartAttackAI();
	}

	private void StartAttackAI()
	{
		_anim.SetBool(IsAttacking, true);

		DOVirtual.DelayedCall(aiDecisionInterval, () =>
												  {
													  if ((_player.position - _transform.position).magnitude > maxDistanceFromPlayer)
														  FindNewPosition();

													  if (Random.value > 0.5f) projectile.StartAttack(_player);
												  }).SetLoops(-1);
	}

	private void FindNewPosition()
	{
		print("too far away");
	}

	private void LaunchProximityReductionAttack()
	{
		proximity.StartAttack(_player);
		
		_inProximityAttackCooldown = true;
		DOVirtual.DelayedCall(proximityCooldownDuration, () => _inProximityAttackCooldown = false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isInCombat) return;
		if(_inProximityAttackCooldown) return;
		if(!other.CompareTag("Player")) return;

		LaunchProximityReductionAttack();
	}
}
