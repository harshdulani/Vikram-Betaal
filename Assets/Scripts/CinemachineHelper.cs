using Betaal;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CinemachineHelper : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera walkCam, fightCam;
	[SerializeField] private float fightCamDistance;

	private CinemachineTrackedDolly _walkCamTracker, _fightCamTracker;
	private Vector3 _initWalkPathOffset, _initFightPathOffset;
		
	private Transform _player, _betaal, _oldie;
	private bool _isUsingFightCam, _isOldieAnEnemy;
	
	private void OnEnable()
	{
		BetaalEvents.StartBetaalAttack += OnStartBetaalAttack;
		BetaalEvents.EndBetaalAttack += OnEndBetaalAttack;
	}

	private void OnDisable()
	{
		BetaalEvents.StartBetaalAttack -= OnStartBetaalAttack;
		BetaalEvents.EndBetaalAttack -= OnEndBetaalAttack;
	}

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_betaal = GameObject.FindGameObjectWithTag("Betaal").transform;
		_oldie = GameObject.FindGameObjectWithTag("Oldie").transform;

		_walkCamTracker = walkCam.GetCinemachineComponent<CinemachineTrackedDolly>();
		_fightCamTracker = fightCam.GetCinemachineComponent<CinemachineTrackedDolly>();

		_initWalkPathOffset = _walkCamTracker.m_PathOffset;
		_initFightPathOffset = _fightCamTracker.m_PathOffset;
		
		//switch between fight and walk cam
		DOVirtual.DelayedCall(0.5f, () =>
									{
										var dist = Vector3.Distance(_player.position, _isOldieAnEnemy ? _oldie .position : _betaal.position);
										if (dist > fightCamDistance)
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
		_isUsingFightCam = true;
		walkCam.m_Priority = 0;
		fightCam.m_Priority = 1;
	}

	private void GoToWalkCam()
	{
		_isUsingFightCam = false;
		walkCam.m_Priority = 1;
		fightCam.m_Priority = 0;
	}

	private void OnStartBetaalAttack()
	{
		DOTween.To(() => _walkCamTracker.m_PathOffset,
				   value => _walkCamTracker.m_PathOffset = value,
				   _walkCamTracker.m_PathOffset + Vector3.right * 2.5f, 1f);
		DOTween.To(() => _fightCamTracker.m_PathOffset,
				   value => _fightCamTracker.m_PathOffset = value,
				   _fightCamTracker.m_PathOffset + Vector3.right * 2.5f, 1f);
	}

	private void OnEndBetaalAttack()
	{
		DOTween.To(() => _walkCamTracker.m_PathOffset,
				value => _walkCamTracker.m_PathOffset = value,
				_initWalkPathOffset, 1f);
		DOTween.To(() => _fightCamTracker.m_PathOffset,
				   value => _fightCamTracker.m_PathOffset = value,
				   _initFightPathOffset, 1f);
	}
}
