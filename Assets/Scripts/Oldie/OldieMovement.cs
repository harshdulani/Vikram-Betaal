using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Oldie
{
	[RequireComponent(typeof (NavMeshAgent))]
	public class OldieMovement : MonoBehaviour
	{
		[SerializeField] private float stoppingDistance;
		private bool _isMoving;
	
		private NavMeshAgent _agent;
		private Transform _transform;

		private OldieRefBank _my;
	
		private static readonly int BlendValue = Animator.StringToHash("blendValue");

		private void Start()
		{
			_agent = GetComponent<NavMeshAgent>();
			_my = GetComponent<OldieRefBank>();

			_transform = transform;
		}

		private void Update()
		{
			Recenter();
			if (Input.GetKeyDown(KeyCode.V)) SetNewDest();
		
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
	}
}