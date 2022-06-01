using UnityEngine;

namespace Player.Combat
{
	public class PlayerCombat : MonoBehaviour
	{
		[SerializeField] private Collider leftHand, rightHand;
		
		private PlayerState _state;
		private Animator _anim;

		//Animator static hashes
		private static readonly int InCombat = Animator.StringToHash("inCombat");
		private static readonly int Light = Animator.StringToHash("light");
		private Transform _leftHandT, _rightHandT;

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

			_state.inCombat = !_state.inCombat;
			_anim.SetBool(InCombat, _state.inCombat);
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