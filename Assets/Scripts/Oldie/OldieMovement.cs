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
		private bool _isMoving;

		private NavMeshAgent _agent;
		private Transform _transform;

		private OldieRefBank _my;

		private static readonly int BlendValue = Animator.StringToHash("blendValue");
		private static readonly int StandUpSitting = Animator.StringToHash("standUpSitting");

		private void OnEnable()
		{
			GameEvents.IntroConversationComplete += OnIntroConversationComplete;
		}

		private void OnDisable()
		{
			GameEvents.IntroConversationComplete -= OnIntroConversationComplete;
		}
		
		private void Start()
		{
			_agent = GetComponent<NavMeshAgent>();
			_my = GetComponent<OldieRefBank>();

			_transform = transform;
			_agent.enabled = false;
			
			sittingSpot.parent = null;
			transform.position = sittingSpot.position;
			transform.rotation = sittingSpot.rotation;
		}

		private void Update()
		{
			if(GameManager.state.InPreFight) return;
			Recenter();
		
			if(!_isMoving) return;
		
			if(Vector3.Distance(_transform.position, _agent.destination) > stoppingDistance) return;

			_agent.isStopped = true;
			StopMovingAnim();
		}

		private void SetNewDest()
		{
			_agent.SetDestination(_transform.position + Vector3.right * (5f * (Random.value > 0.5f ? 1f : -1f)));
			_isMoving = true;
			StartMovingAnim();
		}

		public void SetNewDest(Vector3 position)
		{
			_agent.SetDestination(position);
			_isMoving = true;
			StartMovingAnim();
		}
		
		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);

		private void StartMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 1f, 0.5f);
		private void StopMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 0f, 0.5f);
		private float BlendValueGetter() => _my.Animator.GetFloat(BlendValue);
		private void BlendValueSetter(float value) => _my.Animator.SetFloat(BlendValue, value);
		
		private void OnIntroConversationComplete() 
		{
			_agent.enabled = true;
			_my.Animator.SetTrigger(StandUpSitting);
		}
	}
}