using UnityEngine;

namespace Player.Combat
{
	public class PlayerCombat : MonoBehaviour
	{
		[SerializeField] private Collider leftHand, rightHand;
		public Transform attackHostPosition;
		
		private PlayerState _state;
		private Animator _anim;
		private Transform _leftHandT, _rightHandT;

		//Animator static hashes
		private static readonly int InCombat = Animator.StringToHash("inCombat");
		private static readonly int Light = Animator.StringToHash("light");

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
		}

		public void OnLightAttackInput()
		{
			if(_state.inCombat)
				_anim.SetTrigger(Light);
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
	}
}