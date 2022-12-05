using DG.Tweening;
using Player;
using UnityEngine;

public class Fight3Trigger : MonoBehaviour
{
	private PlayerController _player;
	private bool _hasStarted;

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasStarted) return;
		if(!GameManager.state.betaalFight1Over) return;
		if(GameManager.state.IsInConversation) return; 

		GameManager.state.IsSadhuEvil = true;
		_hasStarted = true;
		Lightning.only.CustomLightning(3500000);

		DOVirtual.DelayedCall(0.15f, () =>
									 {
										 _player.transform.position = transform.position;
										 _player.transform.rotation = Quaternion.LookRotation(Vector3.left);
										 _player.StopCarryingBetaal();
										 _player.HideBetaal();
									 });
		
		
		GameManager.state.startFightAfterNextConversation = true;
		GameManager.state.betaalProperlyDead = true;
		GameEvents.InvokeConversationStart();
	}
}
