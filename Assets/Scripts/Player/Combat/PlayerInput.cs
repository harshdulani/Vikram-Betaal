using System;
using UnityEngine;

namespace Player.Combat
{
	public class PlayerInput : MonoBehaviour
	{
		public static event Action UsePressed;
		public static void InvokeUsePressed() => UsePressed?.Invoke();

		[SerializeField] private Collider leftHand, rightHand;
		public Transform attackHostPosition;

		private PlayerState _state;
		private Animator _anim;
		private Transform _leftHandT, _rightHandT;

		//Animator static hashes
		private static readonly int InCombat = Animator.StringToHash("inCombat");
		private static readonly int Light = Animator.StringToHash("light");
		private static readonly int IsBlocking = Animator.StringToHash("isBlocking");

		private void OnEnable()
		{
			GameEvents.BetaalFightStart += OnFightStart;
			GameEvents.SadhuFightStart += OnFightStart;
			GameEvents.BetaalFightEnd += OnFightEnd;
		}

		private void OnDisable()
		{
			GameEvents.BetaalFightStart -= OnFightStart;
			GameEvents.SadhuFightStart -= OnFightStart;
			GameEvents.BetaalFightEnd -= OnFightEnd;
		}

		private void Start()
		{
			_state = GetComponent<PlayerState>();
			CheckAndInitAnimator();

			_leftHandT = leftHand.transform;
			_rightHandT = rightHand.transform;
		}

		public void SetInCombatStatus(bool status)
		{
			if (status == _state.inCombat) return;

			_state.inCombat = status;
			_anim.SetBool(InCombat, status);

			if (!status) _anim.SetBool(IsBlocking, false);
		}

		public void TurnFistsTriggers()
		{
			leftHand.attachedRigidbody.isKinematic = rightHand.attachedRigidbody.isKinematic = true;
			leftHand.isTrigger = rightHand.isTrigger = true;
			_leftHandT.localPosition = _rightHandT.localPosition = Vector3.zero;
		}

		public void TurnFistsColliders()
		{
			leftHand.attachedRigidbody.isKinematic = rightHand.attachedRigidbody.isKinematic = false;
			leftHand.isTrigger = rightHand.isTrigger = false;
			_leftHandT.localPosition = _rightHandT.localPosition = Vector3.zero;
		}

		public void OnLightAttackInput()
		{
			if (GameManager.state.IsInConversation) return;
			if (_state.inCombat) _anim.SetTrigger(Light);
		}

		public void OnStartBlocking()
		{
			if (GameManager.state.IsInConversation) return;
			if (!_state.inCombat) return;

			CheckAndInitAnimator();
			_anim.SetBool(IsBlocking, true);
			_state.isBlocking = true;
			_state.DisallowMovement();
		}

		public void OnStopBlocking()
		{
			if (GameManager.state.IsInConversation) return;
			if (!_state.inCombat) return;

			CheckAndInitAnimator();
			_anim.SetBool(IsBlocking, false);
			_state.isBlocking = false;
			_state.AllowMovement();
		}

		private void CheckAndInitAnimator()
		{
			if (!_anim) _anim = GetComponent<Animator>();
		}
		
		private void OnFightStart() => SetInCombatStatus(true);

		private void OnFightEnd(bool isTemporary) => SetInCombatStatus(isTemporary);
	}
}