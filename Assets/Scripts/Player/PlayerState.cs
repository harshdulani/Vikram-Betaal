using Player.Combat;
using UnityEngine;

public enum PlayerAttackType { None, LPunch, LUppercut }

namespace Player
{
	public class PlayerState : MonoBehaviour
	{
		public PlayerCombat Combat { get; private set; }
		public PlayerController Controller { get; private set; }

		public PlayerAttackType CurrentAttackType { get; set; }
		
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
