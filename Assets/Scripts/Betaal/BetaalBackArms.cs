using DG.Tweening;
using Player;
using UnityEngine;

namespace Betaal
{
	public class BetaalBackArms : MonoBehaviour
	{
		public static float AttackDuration = 0f;
		
		[SerializeField] private Transform leftIkTarget, rightIkTarget;
		
		[SerializeField] private Transform leftCeiling, rightCeiling;

		[Header("IK Anims"), SerializeField] private AnimationCurve snakeCurve;
		[SerializeField] private Transform leftAnimTarget, rightAnimTarget;

		[HideInInspector] public bool isAttacking;
		private Transform _currentLeftTarget, _currentRightTarget;

		private PlayerState _player;

		private void Start()
		{
			_currentLeftTarget = leftAnimTarget;
			_currentRightTarget = rightAnimTarget;

			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
		}

		private void LateUpdate()
		{
			if(isAttacking) return;
			leftIkTarget.position = Vector3.Lerp(leftIkTarget.position, _currentLeftTarget.position, Time.deltaTime);
			rightIkTarget.position = Vector3.Lerp(rightIkTarget.position, _currentRightTarget.position, Time.deltaTime);
		}

		public void SendToCeiling()
		{
			leftIkTarget.DOMove(leftCeiling.position, 0.5f).OnComplete(() => _currentLeftTarget = leftCeiling);
			rightIkTarget.DOMove(rightCeiling.position, 0.5f).OnComplete(() => _currentRightTarget = rightCeiling);
		}

		public void AttackChest()
		{
			AttackDuration = 0f;
			isAttacking = true;
			BetaalEvents.InvokeStartBetaalAttack();

			var selectedHand = rightIkTarget;

			var position = selectedHand.position;
			var endPos = _player.chestCollider.position;
			
			var direction = endPos - position;
			var chargeUpPosition = position;
			chargeUpPosition += direction.normalized * -1f;
			
			var perpendicular = direction.normalized;
			perpendicular = new Vector3(-perpendicular.y, perpendicular.x, perpendicular.z);
			chargeUpPosition += perpendicular;

			//Debug.DrawLine(position, chargeUpPosition, Color.red, 2f, false);

			AttackDuration += 1f;
			AttackDuration += 0.5f;
			AttackDuration += 0.25f;
			AttackDuration += 1.5f;
			AttackDuration += 0.75f;
			
			selectedHand.DOMove(chargeUpPosition, 1f)
						.SetTarget(this)
						.OnComplete(() => selectedHand.DOMove(endPos, 0.25f)
													  .SetEase(snakeCurve)
													  .SetTarget(this)
													  .SetDelay(0.5f)
													  .OnComplete(() => selectedHand.DOMove(_currentRightTarget.position, 1.5f)
																					.SetTarget(this)
																					.SetDelay(0.75f)
																					.OnComplete(() =>
																								{
																									isAttacking = false;
																									BetaalEvents.InvokeEndBetaalAttack();
																								})));
		}
	}
}