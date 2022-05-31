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

		private void Start()
		{
			_state = GetComponent<PlayerState>();
			_anim = GetComponent<Animator>();
		}

		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.R)) _anim.enabled = !_anim.isActiveAndEnabled;
			if(!Input.GetKeyUp(KeyCode.K)) return;

			_state.inCombat = !_state.inCombat;
			_anim.SetBool(InCombat, _state.inCombat);
		}

		public void OnLightAttackInput()
		{
			if(_state.inCombat)
				_anim.SetTrigger(Light);
		}

		public void TurnFistsTriggers() => leftHand.isTrigger = rightHand.isTrigger = true;

		public void TurnFistsColliders() => leftHand.isTrigger = rightHand.isTrigger = false;
	}
}