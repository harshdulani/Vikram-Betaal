using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Player.Movement
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private Transform sittingSpot;
		[SerializeField] private float magnitudeLerpSpeed = 7f, movementSpeed;
		public bool IsRunning { private get; set; }

		private PlayerState _state;
		private NavMeshAgent _agent;
		private Animator _anim;
		private Transform _transform;

		private float _inputMag;
		private bool _isFacingRight = true;

		private Tween _orientationTween;

		private static readonly int InputMag = Animator.StringToHash("inputMag");
		private static readonly int GetShockedSitting = Animator.StringToHash("getShockedSitting");
		private static readonly int WakeUpSitting = Animator.StringToHash("wakeUpSitting");
		private static readonly int StandUpSitting = Animator.StringToHash("standUpSitting");

		private void OnEnable()
		{
			GameEvents.GameplayStart += OnGameplayStart;
			GameEvents.IntroConversationComplete += OnIntroConversationComplete;
		}

		private void OnDisable()
		{
			GameEvents.GameplayStart -= OnGameplayStart;
			GameEvents.IntroConversationComplete -= OnIntroConversationComplete;
		}

		private void Start()
		{			
			_state = GetComponent<PlayerState>();
			_anim = GetComponent<Animator>();
			_agent = GetComponent<NavMeshAgent>();
			_transform = transform;
			
			_agent.enabled = false;
			
			sittingSpot.parent = null;
			transform.position = sittingSpot.position;
			transform.rotation = sittingSpot.rotation;
			
			_isFacingRight = Vector3.Dot(_transform.forward, Vector3.right) > 0;
		}

		public void Execute(Vector2 input)
		{
			if(GameManager.state.InPreFight) return;
			
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

		private void Move() => _transform.position += Vector3.right * ((_isFacingRight ? 1 : -1) * _inputMag * movementSpeed * Time.deltaTime);

		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);

		private void SetAnimValues() => _anim.SetFloat(InputMag, _inputMag);

		public void EndWakingUpOnAnimation()
		{
			_anim.SetTrigger(WakeUpSitting);

			DOTween.To(() => _anim.GetLayerWeight(2), value => _anim.SetLayerWeight(2, value),
					   1f, 1f);
			GameEvents.InvokeConversationStart();
		}

		private void OnGameplayStart()
		{
			_anim.SetTrigger(GetShockedSitting);
		}

		private void OnIntroConversationComplete() 
		{
			_anim.SetTrigger(StandUpSitting);
			_state.DisableMovementByAnimationStatus();
			
			transform.DOMoveZ(0f, 1.5f).SetEase(Ease.InQuart);
			_transform.DORotate(Vector3.up * 90f, 1f)
					  .SetDelay(1.5f)
					  .OnComplete(() => {
									  _state.EnableMovementByAnimationStatus();
									  _agent.enabled = true;
								  });
		}
	}
}