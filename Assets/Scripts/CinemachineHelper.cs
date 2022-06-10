using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CinemachineHelper : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera walkCam, fightCam;
	[SerializeField] private float fightCamDistance;
	
	private Transform _player, _betaal;
	private bool _isUsingFightCam;

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_betaal = GameObject.FindGameObjectWithTag("Betaal").transform;

		DOVirtual.DelayedCall(0.2f, () =>
									{
										if (Vector3.Distance(_player.position, _betaal.position) > fightCamDistance)
										{
											if(!_isUsingFightCam) return;
											GoToWalkCam();
										}
										else
										{
											if(_isUsingFightCam) return;
											GoToFightCam();
										}
									}).SetLoops(-1);
	}

	private void GoToFightCam()
	{
		walkCam.m_Priority = 0;
		fightCam.m_Priority = 1;
	}

	private void GoToWalkCam() 
	{
		walkCam.m_Priority = 1;
		fightCam.m_Priority = 0;
	}
}
