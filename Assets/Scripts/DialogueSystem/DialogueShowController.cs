using System;
using DG.Tweening;
using Player.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueShowController : MonoBehaviour
{
	[Header("Text"), SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TextMeshProUGUI leftCharacterName, rightCharacterName, dialogue;

	[Header("Image"), SerializeField] private GameObject leftPivotPanel;
	[SerializeField] private GameObject rightPivotPanel;
	[SerializeField] private Image leftImage, rightImage;
	[SerializeField] private Sprite vikramIcon, betaalIcon, sadhuIcon, sadhuEvilIcon;
	
	[Header("Tip"), SerializeField] private TextMeshProUGUI tip;
	[SerializeField] private float blinkDuration, blinkWaitTime;

	private DialogueBank _dialogue;
	[SerializeField] private int _currentPlayerIndex, _currentBetaalIndex, _currentSadhuIndex, _currentConversationIndex;

	private Tween _tipBlinker, _tipWaiter, _tipAppear, _tipDisappear;
	
	private const string GOTOVK = "GOTOVK";
	private const string GOTOBT = "GOTOBT";
	private const string GOTOSD = "GOTOSD";
	private const string EXIT = "EXIT";

	private void OnEnable()
	{
		PlayerInput.UsePressed += OnUsePressed;
		
		GameEvents.ConversationStart += OnConversationStart;
	}

	private void OnDisable()
	{
		PlayerInput.UsePressed -= OnUsePressed;
		
		GameEvents.ConversationStart -= OnConversationStart;
	}

	private void Start()
	{
		_dialogue = GetComponent<DialogueBank>();

		float GetCharSpacing() => tip.characterSpacing;
		void SetCharSpacing(float value) => tip.characterSpacing = value;

		_tipAppear = tip.DOColor(Color.white, 0.05f).SetRecyclable(true).SetAutoKill(false);
		_tipDisappear = tip.DOColor(Color.clear, 0.05f).SetRecyclable(true).SetAutoKill(false);
		
		_tipBlinker = DOTween.To(GetCharSpacing, SetCharSpacing, -36, blinkDuration)
		//_tipBlinker = tip.DOColor(Color.clear, blinkDuration)
						 .SetLoops(-1, LoopType.Yoyo)
						 .SetRecyclable(true)
						 .SetAutoKill(false);

		_tipWaiter = DOVirtual.DelayedCall(blinkWaitTime, RestartTipBlinker)
							  .SetRecyclable(true)
							  .SetAutoKill(false);;

		_tipBlinker.Pause();
		_tipWaiter.Pause();
		_tipAppear.Pause();
		_tipDisappear.Pause();
	}

	private void SpawnDialogSpeaker(DialogueBank.Character character, string currentDialogue)
	{
		dialoguePanel.SetActive(true);
		leftPivotPanel.SetActive(true);
		rightPivotPanel.SetActive(true);
		
		SetCharacter(character);
		SetText(currentDialogue);
	}

	private void SetCharacter(DialogueBank.Character speaker)
	{
		SetSpeakerDetailsPos(speaker, out var characterName, out var icon);

		characterName.text = speaker switch
							 {
								 DialogueBank.Character.Player => "Vikram",
								 DialogueBank.Character.Betaal => "Betaal",
								 DialogueBank.Character.Oldie => "Sadhu",
								 _ => dialogue.text
							 };

		icon.sprite = speaker switch
					  {
						  DialogueBank.Character.Player => vikramIcon,
						  DialogueBank.Character.Betaal => betaalIcon,
						  DialogueBank.Character.Oldie => GameManager.state.isSadhuEvil ? sadhuEvilIcon : sadhuIcon,
						  _ => icon.sprite
					  };
	}

	private bool TryShowNextDialog()
	{
		ref var currentSpeakerIndex = ref _currentPlayerIndex;
		
		//you cant pass ref locals as ref parameters to methods :c
		//c++, here i come c:
		
		currentSpeakerIndex = ref UpdateActiveSpeakerIndex();

		var currentDialogue = _dialogue.GetDialogue(GameManager.state.ActiveSpeaker, currentSpeakerIndex++);
		
		var shouldSkipDialogue = false;

		switch (currentDialogue)
		{
			case GOTOVK:
				GameManager.state.ActiveSpeaker = DialogueBank.Character.Player;
				shouldSkipDialogue = true;
				break;
			case GOTOBT:
				GameManager.state.ActiveSpeaker = DialogueBank.Character.Betaal;
				shouldSkipDialogue = true;
				break;
			case GOTOSD:
				GameManager.state.ActiveSpeaker = DialogueBank.Character.Oldie;
				shouldSkipDialogue = true;
				break;
			case EXIT:
				EndConversation();
				return true;
			default: break;
		}

		print($"{string.Compare(EXIT, currentDialogue)} {currentDialogue}");
		if (shouldSkipDialogue)
		{
			currentSpeakerIndex = ref UpdateActiveSpeakerIndex();
			currentDialogue = _dialogue.GetDialogue(GameManager.state.ActiveSpeaker, currentSpeakerIndex++);
		}
		
		SpawnDialogSpeaker(GameManager.state.ActiveSpeaker, currentDialogue);
		return false;
	}

	private ref int UpdateActiveSpeakerIndex()
	{
		ref var currentIdx = ref _currentPlayerIndex;
		switch (GameManager.state.ActiveSpeaker)
		{
			case DialogueBank.Character.Player: break;
			case DialogueBank.Character.Betaal: currentIdx = ref _currentBetaalIndex;
				break;
			case DialogueBank.Character.Oldie: currentIdx = ref _currentSadhuIndex;
				break;
		}

		return ref currentIdx;
	}

	private void EndConversation()
	{
		print("end");
		dialoguePanel.SetActive(false);
		leftPivotPanel.SetActive(false);
		rightPivotPanel.SetActive(false);
		
		if(_currentConversationIndex++ == 0)
			GameEvents.InvokeIntroConversationComplete();
	}

	private void SetSpeakerDetailsPos(DialogueBank.Character speaker, out TextMeshProUGUI text, out Image image)
	{
		var convWithBetaal = GameManager.state.InConversationWithBetaal;
		if (speaker == DialogueBank.Character.Player)
			if (convWithBetaal)
				SetLeftIcon(out text, out image);
			else
				SetRightIcon(out text, out image);
		else if (speaker == DialogueBank.Character.Betaal)
			SetRightIcon(out text, out image);
		else//if (speaker == DialogueBank.Character.Oldie) 
			SetLeftIcon(out text, out image);
	}

	private void SetText(string text) => dialogue.text = text;

	private void ProgressConversation()
	{
		if(TryShowNextDialog()) return;
		
		if (_tipBlinker.IsActive())
		{
			_tipBlinker.Pause();
			tip.color = Color.clear;
		}

		HideTipBlinker();
		if(_tipWaiter.IsActive()) _tipWaiter.Kill();
		
		_tipWaiter.Restart();
	}

	private void SetLeftIcon(out TextMeshProUGUI text, out Image image)
	{
		image = leftImage;
		text = rightCharacterName;
	}

	private void SetRightIcon(out TextMeshProUGUI text, out Image image)
	{
		image = rightImage;
		text = leftCharacterName;
	}

	private void RestartTipBlinker()
	{
		_tipAppear.Restart();
		_tipBlinker.Restart();
	}

	private void HideTipBlinker() => _tipDisappear.Restart();

	private void OnConversationStart() => ProgressConversation();

	private void OnUsePressed()
	{
		if(!GameManager.state.IsInConversation) return;
		ProgressConversation();
	}
}