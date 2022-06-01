using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ConcreteBlocksMoving : MonoBehaviour
{
	[SerializeField] private float oneTweenTime = 5f;
	private RandomiseTrees _treeRandomiser;
	private CinemachineImpulseSource _impulse;

	private void Start()
	{
		_treeRandomiser = GetComponent<RandomiseTrees>();
		_impulse = GetComponent<CinemachineImpulseSource>();
		
		transform.DOMoveX(0f, oneTweenTime)
				 .SetLoops(-1, LoopType.Restart)
				 .SetEase(Ease.Linear)
				 .OnStepComplete(() =>
								 {
									 _treeRandomiser.Randomise();
								 });
	}
}
