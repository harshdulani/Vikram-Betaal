using TMPro;
using UnityEngine;

public class DialogueShowController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI characterName, dialogue;

	private Dialogue _dialogue;
	private int _currentPlayerIndex, _currentBetaalIndex, _currentSadhuIndex;

	private void Start()
	{
		_dialogue = GetComponent<Dialogue>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q)) SpawnDialog();
	}

	private void SpawnDialog()
	{
		SetCharacter(Dialogue.Character.Player);
		SetText(_dialogue.GetDialogue(Dialogue.Character.Player, _currentPlayerIndex));
	}
	
	private void SetCharacter(Dialogue.Character character)
	{
		characterName.text = character switch
							 {
								 Dialogue.Character.Player => "Vikram",
								 Dialogue.Character.Betaal => "Betaal",
								 Dialogue.Character.Oldie => "Sadhu",
								 _ => dialogue.text
							 };
	}

	private void SetText(string text)
	{
		dialogue.text = text;
	}
}