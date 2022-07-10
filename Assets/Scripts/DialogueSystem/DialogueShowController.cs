using TMPro;
using UnityEngine;

public class DialogueShowController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI characterName, dialogue;

	private DialogueBank _dialogue;
	private int _currentPlayerIndex, _currentBetaalIndex, _currentSadhuIndex;

	private void Start()
	{
		_dialogue = GetComponent<DialogueBank>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q)) SpawnDialog();
	}

	private void SpawnDialog()
	{
		SetCharacter(DialogueBank.Character.Player);
		SetText(_dialogue.GetDialogue(DialogueBank.Character.Player, _currentPlayerIndex));
	}
	
	private void SetCharacter(DialogueBank.Character character)
	{
		characterName.text = character switch
							 {
								 DialogueBank.Character.Player => "Vikram",
								 DialogueBank.Character.Betaal => "Betaal",
								 DialogueBank.Character.Oldie => "Sadhu",
								 _ => dialogue.text
							 };
	}

	private void SetText(string text)
	{
		dialogue.text = text;
	}
}