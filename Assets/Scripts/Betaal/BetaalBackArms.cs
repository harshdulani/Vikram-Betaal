using DG.Tweening;
using Player;
using UnityEngine;

namespace Betaal
{
	public class BetaalBackArms : MonoBehaviour
	{
		[SerializeField] private Transform leftIkTarget, rightIkTarget;
		
		[SerializeField] private Transform leftCeiling, rightCeiling;

		[Header("IK Anims"), SerializeField] private AnimationCurve snakeCurve;
		[SerializeField] private Transform leftAnimTarget, rightAnimTarget;

		private Transform _currentLeftTarget, _currentRightTarget;

		private PlayerState _player;
		private bool _isAttacking;

		private void Start()
		{
			_currentLeftTarget = leftAnimTarget;
			_currentRightTarget = rightAnimTarget;

			_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
		}

		private void Update()
		{
			if(Input.GetKeyUp(KeyCode.Backspace)) AttackChest();
		}

		private void LateUpdate()
		{
			if(_isAttacking) return;
			leftIkTarget.position = Vector3.Lerp(leftIkTarget.position, _currentLeftTarget.position, Time.deltaTime);
			rightIkTarget.position = Vector3.Lerp(rightIkTarget.position, _currentRightTarget.position, Time.deltaTime);
		}

		public void SendToCeiling()
		{
			leftIkTarget.DOMove(leftCeiling.position, 0.5f).OnComplete(() => _currentLeftTarget = leftCeiling);
			rightIkTarget.DOMove(rightCeiling.position, 0.5f).OnComplete(() => _currentRightTarget = rightCeiling);
		}

		private void AttackChest()
		{
			_isAttacking = true;
			BetaalEvents.InvokeStartBetaalAttack();

			var rand = Random.value;
			//var selectedHand = rand > 0.5f ? leftIkTarget : rightIkTarget;
			
			var selectedHand = rightIkTarget;

			var position = selectedHand.position;
			var endPos = _player.chestCollider.position;
			
			var direction = endPos - position;
			var chargeUpPosition = position;
			chargeUpPosition += direction.normalized * -1f;
			
			var perpendicular = direction.normalized;
			perpendicular = new Vector3(-perpendicular.y, perpendicular.x, perpendicular.z);
			chargeUpPosition += perpendicular;

			Debug.DrawLine(position, chargeUpPosition, Color.red, 2f, false);

			selectedHand.DOMove(chargeUpPosition, 1f)
						.OnComplete(() => selectedHand.DOMove(endPos, 0.25f)
													  .SetEase(snakeCurve)
													  .SetDelay(0.5f)
													  .OnComplete(() => selectedHand.DOMove(_currentRightTarget.position, 1.5f)
																					.OnComplete(() =>
																								{
																									_isAttacking = false;
																									BetaalEvents.InvokeEndBetaalAttack();
																								})));
		}
	}
}