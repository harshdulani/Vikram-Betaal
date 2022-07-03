﻿using DG.Tweening;
using UnityEngine;

namespace Player.Movement
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private float magnitudeLerpSpeed = 7f, movementSpeed;
		public bool IsRunning { private get; set; }

		private PlayerState _state;
		private Animator _anim;
		private Transform _transform;

		private float _inputMag;
		private bool _isFacingRight = true;

		private Tween _orientationTween;
		
		private static readonly int InputMag = Animator.StringToHash("inputMag");

		private void Start()
		{			
			_state = GetComponent<PlayerState>();
			_anim = GetComponent<Animator>();
			_transform = transform;

			_isFacingRight = Vector3.Dot(_transform.forward, Vector3.right) > 0;
		}
		
		public void Execute(Vector2 input)
		{
			Recenter();
			if (_state.disableMovementByAnimation)
			{
				_inputMag = Mathf.Lerp(_inputMag, 0f, magnitudeLerpSpeed * 3 * Time.deltaTime);
				SetAnimValues();
				return;
			}
			
			var magnitude = input.magnitude * (IsRunning && !_state.inCombat ? 3 : 1);

			_inputMag = Mathf.Lerp(_inputMag, magnitude, magnitudeLerpSpeed * Time.deltaTime);
			SetAnimValues();

			if(magnitude < 0.01f) return;
			
			HandleRotation(input.x);
			Move();
		}

		public void UpdateRunningStatus(bool status)
		{
			IsRunning = status;

			if (status) _state.Combat.SetInCombatStatus(false);
		}

		private void HandleRotation(float inputX)
		{
			var shouldFaceRight = inputX > 0;
			
			if(shouldFaceRight == _isFacingRight) return;
			
			if(_orientationTween != null && _orientationTween.IsActive()) _orientationTween.Kill();

			_orientationTween = _transform.DORotate(Vector3.up * (90 * inputX), 0.25f);
			_isFacingRight = shouldFaceRight;
		}

		private void Move()
		{
			_transform.position += Vector3.right * ((_isFacingRight ? 1 : -1) * _inputMag * movementSpeed * Time.deltaTime);
		}

		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);

		private void SetAnimValues() => _anim.SetFloat(InputMag, _inputMag);
	}
}