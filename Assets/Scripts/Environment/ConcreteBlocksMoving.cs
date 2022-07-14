using DG.Tweening;
using UnityEngine;

public class ConcreteBlocksMoving : MonoBehaviour
{
	[SerializeField] private float oneTweenTime = 5f;
	private RandomiseTrees _treeRandomiser;

	private void Start()
	{
		_treeRandomiser = GetComponent<RandomiseTrees>();
		
		transform.DOLocalMoveX(0f, oneTweenTime)
				 .SetLoops(-1, LoopType.Restart)
				 .SetEase(Ease.Linear)
				 .OnStepComplete(() =>
								 {
									 _treeRandomiser.Randomise();
								 });
	}
}
