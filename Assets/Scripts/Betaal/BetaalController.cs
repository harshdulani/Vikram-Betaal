using System;
using DG.Tweening;
using Player;
using Player.Combat;
using UnityEngine;

public class BetaalController : MonoBehaviour
{
	[SerializeField] private int maxHealth;
	
	[SerializeField] private Rigidbody[] rigidbodies;
	[SerializeField] private float ragdollThrowBackForce;

	private Animator _anim;
	private Transform _transform;

	private int _currentHealth;
	private static readonly int HitPunch = Animator.StringToHash("hitPunch");
	private static readonly int HitUppercut = Animator.StringToHash("hitUppercut");
	private static readonly int Dummy = Animator.StringToHash("dummy");

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_transform = transform;
		_currentHealth = maxHealth;
	}

	private void Update()
	{
		Recenter();
	}

	public void GiveDamage(int getAttackDamage, AttackType type)
	{
		_currentHealth -= getAttackDamage;
		_anim.SetTrigger(type switch
						 {
							 AttackType.LPunch => HitPunch,
							 AttackType.LUppercut => HitUppercut,
							 _ => Dummy
						 });
		
		print(_currentHealth);
		if(_currentHealth > 0) return;
		
		_anim.enabled = false;
		GoRagdoll();
	}

	private void GoRagdoll()
	{
		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
			rb.AddForce(-_transform.forward * ragdollThrowBackForce + Vector3.up * ragdollThrowBackForce * 20, ForceMode.Impulse);
		}
	}

	private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);
}
