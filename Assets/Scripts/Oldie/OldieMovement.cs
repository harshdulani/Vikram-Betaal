using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Oldie
{
	[RequireComponent(typeof (NavMeshAgent))]
	public class OldieMovement : MonoBehaviour
	{
		[SerializeField] private Transform sittingSpot;
		[SerializeField] private float stoppingDistance;
		private bool _isMoving, _disabledMovement;

		private NavMeshAgent _agent;
		private Transform _transform;
		private Transform _player;
		
		private OldieRefBank _my;

		private static readonly int BlendValue = Animator.StringToHash("blendValue");
		private static readonly int StandUpSitting = Animator.StringToHash("standUpSitting");

		private void OnEnable()
		{
			GameEvents.ConversationStart += OnConversationStart;
		}

		private void OnDisable()
		{
			GameEvents.ConversationStart -= OnConversationStart;
		}

		private void Start()
		{
			_agent = GetComponent<NavMeshAgent>();
			_my = GetComponent<OldieRefBank>();
			_player = GameObject.FindGameObjectWithTag("Player").transform.root;

			_transform = transform;
			_agent.enabled = false;
			
			sittingSpot.parent = null;
			_transform.position = sittingSpot.position;
			_transform.rotation = sittingSpot.rotation;
		}

		private void Update()
		{
			if(_disabledMovement) return;
			if(GameManager.state.InPreFight) return;
			Recenter();
		
			if(!_isMoving) return;
		
			if(Vector3.Distance(_transform.position, _agent.destination) > stoppingDistance) return;

			_isMoving = false;
			_agent.isStopped = true;
			StopMovingAnim();
		}

		public void SetNewDest(Vector3 position)
		{
			_agent.SetDestination(position);
			_isMoving = true;
			StartMovingAnim();
		}

		public void GetReadyForFight()
		{
			_agent.enabled = true;
			
			_my.Animator.SetTrigger(StandUpSitting);
			_transform.DOMoveZ(0f, 1.5f).SetEase(Ease.InQuart)
				.OnComplete(() => 
					            _transform
						            .DORotateQuaternion(Quaternion.LookRotation(_player.position - _transform.position), 0.25f));
		}

		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);

		private void StartMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 1f, 0.5f);
		private void StopMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 0f, 0.5f);
		private float BlendValueGetter() => _my.Animator.GetFloat(BlendValue);
		private void BlendValueSetter(float value) => _my.Animator.SetFloat(BlendValue, value);

		private void OnConversationStart() => _disabledMovement = true;
	}
}