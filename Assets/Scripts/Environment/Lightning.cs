using DG.Tweening;
using UnityEngine;

public class Lightning : MonoBehaviour
{
	public static Lightning only;
	
	[SerializeField] private float peakIntensity;
	[SerializeField] private Vector2 waitRange;
	private Light _sun;

	private Tween _lightningTween;

	private void Awake()
	{
		if (only) Destroy(gameObject);
		else only = this;
	}

	private void Start()
	{
		_sun = GetComponent<Light>();

		_lightningTween = _sun.DOIntensity(peakIntensity, 0.15f)
							  .SetEase(Ease.Flash)
							  .SetLoops(4, LoopType.Yoyo)
							  .SetDelay(Random.Range(0, 1f))
							  .OnComplete(() => _lightningTween.Restart(true, Random.Range(waitRange.x, waitRange.y)));
	}

	public void CustomLightning(float luxValue)
	{
		_lightningTween.Rewind();

		_sun.DOIntensity(luxValue, 0.15f)
			.SetLoops(2, LoopType.Yoyo)
			.SetEase(Ease.Flash)
			.OnComplete(() =>
							_sun.DOIntensity(luxValue, 0.15f)
								.SetLoops(2, LoopType.Yoyo)
								.SetEase(Ease.Flash)
								.SetDelay(Random.Range(0, 1f))
								.OnComplete(() => _lightningTween.Restart(true, Random.Range(waitRange.x, waitRange.y))));
	}
}