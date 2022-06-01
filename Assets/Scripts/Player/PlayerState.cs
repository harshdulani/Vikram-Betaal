using Player.Combat;
using UnityEngine;

namespace Player
{	
	public enum AttackType { None, LPunch, LUppercut }

	public class PlayerState : MonoBehaviour
	{
		public PlayerCombat Combat { get; private set; }
		public PlayerController Controller { get; private set; }

		public AttackType CurrentAttackType { get; set; }
		
		public bool inCombat, disableMovementByAnimation;
		
		public void EnableMovementByAnimationStatus()  => disableMovementByAnimation = false;
		public void DisableMovementByAnimationStatus()  => disableMovementByAnimation = true;


		private void Start()
		{
			Combat = GetComponent<PlayerCombat>();
			Controller = GetComponent<PlayerController>();
		}
	}
}
