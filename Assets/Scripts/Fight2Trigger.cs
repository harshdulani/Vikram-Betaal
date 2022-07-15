using Betaal;
using DG.Tweening;
using Player;
using UnityEngine;

public class Fight2Trigger : MonoBehaviour
{
	private PlayerController _player;
	[SerializeField] private BetaalController betaal;
	private bool _hasStarted;

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform.root.GetComponent<PlayerController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasStarted) return;
		if(!GameManager.state.betaalFight1Over) return;

		_hasStarted = true;
		Lightning.only.CustomLightning(3500000);

		DOVirtual.DelayedCall(0.15f, () =>
									 {
										 _player.transform.position = transform.position - Vector3.right * 5f;
										 _player.transform.rotation = Quaternion.LookRotation(Vector3.right);
										 _player.StopCarryingBetaal();
										 betaal.transform.position = transform.position;
										 betaal.gameObject.SetActive(true);
										 betaal.DeRagdoll();
									 });
		
		GameManager.state.startFightAfterNextConversation = true;
		GameEvents.InvokeConversationStart();
	}
}
