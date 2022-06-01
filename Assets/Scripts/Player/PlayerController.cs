using System;
using UnityEngine;

namespace Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private int lPunchDamage = 20, lUppercutDamage = 40;
		
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