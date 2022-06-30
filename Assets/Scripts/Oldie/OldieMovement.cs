using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class OldieMovement : MonoBehaviour
{
	[SerializeField] private float stoppingDistance;
	private bool _isMoving;
	
	private NavMeshAgent _agent;
	private Animator _anim;
	private Transform _transform;

	private static readonly int BlendValue = Animator.StringToHash("blendValue");

	private void Start()
	{
		_agent = GetComponent<NavMeshAgent>();
		_anim = GetComponent<Animator>();

		_transform = transform;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.V)) NewDest();
		
		if(!_isMoving) return;
		
		if(Vector3.Distance(_transform.position, _agent.destination) > stoppingDistance) return;

		_agent.isStopped = true;
		StopMovingAnim();
	}


	private void NewDest()
	{
		_agent.SetDestination(_transform.position + Vector3.right * (5f * (Random.value > 0.5f ? 1f : -1f)));
		_isMoving = true;
		StartMovingAnim();
	}

	private float BlendValueGetter() => _anim.GetFloat(BlendValue);
	private void BlendValueSetter(float value) => _anim.SetFloat(BlendValue, value);
	
	private void StartMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 1f, 0.5f);
	
	private void StopMovingAnim() => DOTween.To(BlendValueGetter, BlendValueSetter, 0f, 0.5f);
}
