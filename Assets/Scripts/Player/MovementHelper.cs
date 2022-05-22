using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class MovementHelper : MonoBehaviour
	{
		private PlayerControls _playerControls;
		private InputAction _movement;

		private PlayerMovement _playerMovement; 

		private void Awake()
		{
			_playerControls = new PlayerControls();
		}

		private void OnEnable()
		{
			_movement = _playerControls.Player.Move;
			_movement.Enable();

			_playerControls.Player.Fire.performed += OnFired;
			_playerControls.Player.Fire.Enable();
			
			_playerControls.Player.Run.performed += OnRunStart;
			_playerControls.Player.Run.Enable();

			_playerControls.Player.Run.canceled += OnRunEnd;
			_playerControls.Player.Run.Enable();
		}

		private void OnDisable()
		{
			_playerControls.Player.Fire.performed -= OnFired;
			_playerControls.Player.Fire.Disable();

			_playerControls.Player.Run.performed -= OnRunStart;
			_playerControls.Player.Run.Disable();

			_playerControls.Player.Run.canceled -= OnRunEnd;
			_playerControls.Player.Run.Disable();
			
			_movement.Disable();
		}

		private void Start()
		{
			_playerMovement = GetComponent<PlayerMovement>();
		}

		private void Update()
		{
			var input = _movement.ReadValue<Vector2>();
			_playerMovement.Execute(input);
		}
		
		private void OnFired(InputAction.CallbackContext context)
		{
			print("Fire");
		}

		private void OnRunStart(InputAction.CallbackContext context)
		{
			print("run");
		}

		private void OnRunEnd(InputAction.CallbackContext context)
		{
			print("run cancel");
		}
	}
}