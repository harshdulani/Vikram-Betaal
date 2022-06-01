using UnityEngine;

namespace Player
{
	public class CombatMoveDisableMovement : StateMachineBehaviour
	{
		[SerializeField] private AttackType myAttackType;
		[SerializeField] private bool shouldDisable;
		private PlayerState _state;
		
		private static readonly int Light = Animator.StringToHash("light");

		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!_state) _state = animator.GetComponent<PlayerState>();

			_state.CurrentAttackType = myAttackType;
			if (shouldDisable)
			{
				_state.DisableMovementByAnimationStatus();
				_state.Combat.TurnFistsColliders();
			}
			else
			{
				_state.EnableMovementByAnimationStatus();
				//animator.ResetTrigger(Light);
				_state.Combat.TurnFistsTriggers();
			}
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
//		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//		{
//		}

		// OnStateMove is called right after Animator.OnAnimatorMove()
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that processes and affects root motion
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK()
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that sets up animation IK (inverse kinematics)
		//}
	}
}