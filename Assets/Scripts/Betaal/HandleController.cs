using DG.Tweening;
using UnityEngine;

namespace Betaal
{
	public class HandleController : MonoBehaviour
	{
		private Vector3 _initPos;
		private bool _isAttacking;
	
		private Rigidbody _rb;
		private Transform _transform;

		private void OnEnable()
		{
			BetaalEvents.StartHandleAttack += OnStartHandleAttack;
		}
	
		private void OnDisable()
		{
			BetaalEvents.StartHandleAttack -= OnStartHandleAttack;
		}

		private void Start()
		{
			_rb = GetComponent<Rigidbody>();
			_transform = transform;
			_initPos = _transform.position;
		}

		public void TryRotate(Vector3 torqueMultiplier, ForceMode velocityChange)
		{
			if(_isAttacking) return;
			_rb.AddTorque(torqueMultiplier, velocityChange);
		}

		private void StopAttacking() => _isAttacking = false;

		private void OnStartHandleAttack(BetaalController betaal)
		{
			if(Vector3.Distance(betaal.transform.position, _transform.position) > betaal.handleAttack.maxDistanceForAttack) return;

			_isAttacking = true;
			_rb.isKinematic = true;
		
			var seq = DOTween.Sequence();
		
			seq.Append(_transform.DOShakeRotation(
												  betaal.handleAttack.shakeTime, 
												  betaal.handleAttack.shakeStrength, 
												  10, 90f, false));
			seq.Join(_transform.DOMoveY(_initPos.y - 0.5f, betaal.handleAttack.shakeTime));

			var startPos = Vector3.zero;
			seq.AppendCallback(() => _rb.isKinematic = false);
			seq.AppendCallback(() => startPos = _transform.position);
		
			seq.Append(_transform.DOMove(betaal.handleAttack.attackPos, 0.5f).SetEase(Ease.InExpo));
			seq.AppendCallback(() => _rb.AddForce((_transform.position - startPos).normalized * 15f, ForceMode.Impulse));

			seq.AppendInterval(betaal.handleAttack.postAttackInterval);
			seq.AppendCallback(() => _rb.isKinematic = true);

			seq.Join(_transform.DOMoveX(_initPos.x, 1f));
			seq.Join(_transform.DOMoveY(_initPos.y, 1.25f).SetEase(Ease.OutExpo));
			seq.Join(_transform.DOMoveZ(_initPos.z, 1f));
			seq.Join(_transform.DORotate(Vector3.zero, 1f));
			seq.AppendInterval(0.1f);
		
			seq.AppendCallback(() => _rb.isKinematic = false);

			seq.AppendCallback(StopAttacking);
		}
	}
}