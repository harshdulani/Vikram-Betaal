using System;
using UnityEngine;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private int lPunchDamage = 20, lUppercutDamage = 40;

		private Animator _anim;

		private void Start()
		{
			_anim = GetComponent<Animator>();
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.R)) _anim.enabled = !_anim.isActiveAndEnabled;
		}

		public int GetAttackDamage(AttackType type)
		{
			return type switch
				   {
					   AttackType.None => 0,
					   AttackType.LPunch => lPunchDamage,
					   AttackType.LUppercut => lUppercutDamage,
					   _ => throw new ArgumentOutOfRangeException(nameof (type), type, null)
				   };
		}
	}
}