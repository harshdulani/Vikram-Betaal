using Oldie;
using DG.Tweening;
using Player;
using UnityEngine;

public class Fight3Trigger : MonoBehaviour
{
	private OldieRefBank _sadhu;
	private PlayerController _player;
	private bool _hasStarted;

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerController>();
		_sadhu = GameObject.FindGameObjectWithTag("Oldie").transform.root.GetComponent<OldieRefBank>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasStarted) return;
		if(!GameManager.state.betaalFight1Over) return;

		GameManager.state.IsSadhuEvil = true;
		_hasStarted = true;
		Lightning.only.CustomLightning(3500000);

		DOVirtual.DelayedCall(0.15f, () =>
									 {
										 _player.transform.position = transform.position - Vector3.right * 5f;
										 _player.transform.rotation = Quaternion.LookRotation(Vector3.right);
										 _player.StopCarryingBetaal();
										 _player.HideBetaal();
										 _sadhu.Combat.StartCombat();
									 });
		
		GameManager.state.startFightAfterNextConversation = true;
		GameEvents.InvokeConversationStart();
	}
}
