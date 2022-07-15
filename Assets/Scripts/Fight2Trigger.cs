using UnityEngine;

public class Fight2Trigger : MonoBehaviour
{
	private bool _hasStarted;

	private void OnTriggerEnter(Collider other)
	{
		if(_hasStarted) return;
		if(GameManager.state.betaalFight1Over) return;

		_hasStarted = true;
		GameEvents.InvokeConversationStart();
	}
}
