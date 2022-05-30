using UnityEngine;

namespace Player
{
	public class PlayerState : MonoBehaviour
	{
		public bool inCombat, disableMovementByAnimation;
		
		public void EnableMovementByAnimationStatus()  => disableMovementByAnimation = false;
		public void DisableMovementByAnimationStatus()  => disableMovementByAnimation = true;
	}
}
