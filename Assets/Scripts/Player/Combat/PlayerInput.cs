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

		private void Start()
		{
			_state = GetComponent<PlayerState>();
			_anim = GetComponent<Animator>();

			_leftHandT = leftHand.transform;
			_rightHandT = rightHand.transform;
		}

		private void Update()
		{
			if(!Input.GetKeyUp(KeyCode.T)) return;

			SetInCombatStatus(!_state.inCombat);
		}

		public void SetInCombatStatus(bool status)
		{
			if(status == _state.inCombat) return;
			
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
			if(_state.inCombat)
				_anim.SetTrigger(Light);
		}

		public void OnStartBlocking()
		{
			if (!_state.inCombat) return;
			
			_anim.SetBool(IsBlocking, true);
			_state.DisableMovementByAnimationStatus();
		}

		public void OnStopBlocking()
		{
			if (!_state.inCombat) return;

			_anim.SetBool(IsBlocking, true);
			_state.isBlocking = true;
			_state.EnableMovementByAnimationStatus();
		}
	}
}