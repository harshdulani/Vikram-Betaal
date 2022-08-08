using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Betaal
{
	public class BetaalMovement : MonoBehaviour
	{
		[HideInInspector] public Vector3 initPos;
		
		public Vector2 distanceFromPlayerRange;
		[SerializeField] private float maxDistanceFromHome, aiRepositionInterval;
		[SerializeField] private float stoppingDistance;
		private bool _isMoving, _disabledMovement;

		private NavMeshAgent _agent;
		private Transform _player, _transform;
		private Tween _movementTween;

		private BetaalController _controller;

		private void Start()
		{
			_controller = GetComponent<BetaalController>();
			_agent = GetComponent<NavMeshAgent>();
			_player = GameObject.FindGameObjectWithTag("Player").transform.root;

			_transform = transform;
		}

		private void Update()
		{
			Recenter();
			
			if(!_isMoving) return;
		
			if(Vector3.Distance(_transform.position, _agent.destination) > stoppingDistance) return;

			_isMoving = false;
			_agent.isStopped = true;
		}

		public void StartMovementTween()
		{
			_movementTween = DOVirtual.DelayedCall(aiRepositionInterval,
												   () =>
												   {
													   if (Vector3.Distance(_transform.position, initPos) > maxDistanceFromHome)
													   {
														   SetNewDest(initPos);
														   return;
													   }

													   var vector = _transform.position - _player.position;

													   //filhaal betaal wants to maintain the same amount of distance from player
													   var dist = vector.magnitude;
													   
													   //if player is getting away
													   if ( dist > distanceFromPlayerRange.y)
													   {
														   FindNewPosition(vector);
													   }
													   else if(dist < distanceFromPlayerRange.x)
													   {
														   //if player is getting too close and you are far away from the 
														   FindNewPosition(vector);
													   }
												   }).SetLoops(-1);
		}

		public void StopMovementTween() => _movementTween.Kill();

		public void AssignNewHomePos() => initPos = _transform.position;

		private void FindNewPosition(Vector3 vector)
		{
			var dest = _player.position + vector.normalized * distanceFromPlayerRange.x;
			
			if(dest.x > initPos.x)
			{
				_controller.StartArmsAttack();
				SetNewDest(initPos);
				return;
			}
			SetNewDest(dest);
		}

		private void SetNewDest(Vector3 position)
		{
			_agent.SetDestination(position);
			_agent.isStopped = false;
			_isMoving = true;
		}

		private void Recenter() => _transform.position = Vector3.Lerp(_transform.position, Vector3.right * _transform.position.x, Time.deltaTime * 10f);
	}
}