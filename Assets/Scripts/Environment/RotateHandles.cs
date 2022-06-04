using System.Collections.Generic;
using Betaal;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class RotateHandles : MonoBehaviour
{
	[SerializeField] private float interval = 7f, torque;
	[SerializeField] private HandleController[] handles;
	[SerializeField] private List<float> torqueMultipliers = new List<float>();
	
	private CinemachineImpulseSource _impulse;
	
	private int _randomIndex;

	private void Start()
	{
		_impulse = GetComponent<CinemachineImpulseSource>();

		DOVirtual.DelayedCall(interval, () =>
										{
											_impulse.GenerateImpulse();
											foreach (var handle in handles)
												handle.TryRotate(Vector3.forward * (torque * (1 + torqueMultipliers[_randomIndex++ % torqueMultipliers.Count])),
																 ForceMode.VelocityChange);
										}).SetLoops(-1, LoopType.Restart);
	}
}