using DG.Tweening;
using UnityEngine;

public class Lightning : MonoBehaviour
{
	[SerializeField] private float peakIntensity;
	[SerializeField] private Vector2 waitRange;
	private Light _sun;

	private Tween _lightningTween;

	private void Start()
	{
		_sun = GetComponent<Light>();

		_lightningTween = _sun.DOIntensity(peakIntensity, 0.15f)
							  .SetEase(Ease.Flash)
							  .SetLoops(4, LoopType.Yoyo)
							  .SetDelay(Random.Range(0, 1f))
							  .OnComplete(() => _lightningTween.Restart(true, Random.Range(waitRange.x, waitRange.y)));
	}
}
