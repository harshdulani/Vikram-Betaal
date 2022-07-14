using System;
using System.Collections.Generic;
using DG.Tweening;
using Player.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueShowController : MonoBehaviour
{
	[SerializeField] private List<DialogueBank> dialogueBanks;
	
	[Header("Text"), SerializeField] private GameObject dialoguePanel;
	[SerializeField] private TextMeshProUGUI leftCharacterName, rightCharacterName, dialogue;

	[Header("Image"), SerializeField] private GameObject leftPivotPanel;
	[SerializeField] private GameObject rightPivotPanel;
	[SerializeField] private Image leftImage, rightImage; 
	[SerializeField] private Sprite vikramIcon, betaalIcon, sadhuIcon, sadhuEvilIcon;
	
	[Header("Tip"), SerializeField] private TextMeshProUGUI tip;
	[SerializeField] private float blinkDuration, blinkWaitTime;

	[SerializeField] private int _currentPlayerIndex, _currentBetaalIndex, _currentSadhuIndex, _currentConversationIndex;
	[SerializeField] private List<Character> firstWordBy;

	private Character _currentLeftCharacter;
	private DialogueBank _currentDialogue;
	private Tween _tipBlinker, _tipWaiter, _tipAppear, _tipDisappear;
	private bool _isInIntroConv;

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
		float GetCharSpacing() => tip.characterSpacing;
		void SetCharSpacing(float value) => tip.characterSpacing = value;

		_tipAppear = tip.DOColor(Color.white, 0.05f).SetRecyclable(true).SetAutoKill(false);
		_tipDisappear = tip.DOColor(Color.clear, 0.05f).SetRecyclable(true).SetAutoKill(false);
		
		_tipBlinker = DOTween.To(GetCharSpacing, SetCharSpacing, -36, blinkDuration)
							 .SetLoops(-1, LoopType.Yoyo)
							 .SetRecyclable(true)
							 .SetAutoKill(false);

		_tipWaiter = DOVirtual.DelayedCall(blinkWaitTime, RestartTipBlinker)
							  .SetRecyclable(true)
							  .SetAutoKill(false);

		_tipBlinker.Pause();
		_tipWaiter.Pause();
		_tipAppear.Pause();
		_tipDisappear.Pause();
	}

	private void SpawnDialogSpeaker(Character character, string currentDialogue)
	{
		dialoguePanel.SetActive(true);
		leftPivotPanel.SetActive(true);
		rightPivotPanel.SetActive(true);
		
		SetCharacter(character);
		SetText(currentDialogue);
	}

	private void SetCharacter(Character speaker)
	{
		SetSpeakerDetailsPos(speaker, out var characterName, out var icon);

		characterName.text = speaker switch
							 {
								 Character.Player => "Vikram",
								 Character.Betaal => "Betaal",
								 Character.Oldie => "Sadhu",
								 _ => dialogue.text
							 };

		icon.sprite = speaker switch
					  {
						  Character.Player => vikramIcon,
						  Character.Betaal => betaalIcon,
						  Character.Oldie => GameManager.state.isSadhuEvil ? sadhuEvilIcon : sadhuIcon,
						  _ => icon.sprite
					  };
	}

	private bool TryShowNextDialog()
	{
		ref var currentSpeakerIndex = ref _currentPlayerIndex;
		
		//you cant pass ref locals as ref parameters to methods :c
		//c++, here i come c:
		
		currentSpeakerIndex = ref UpdateActiveSpeakerIndex();
		
		var currentDialogue = _currentDialogue.GetDialogue(GameManager.state.ActiveSpeaker, currentSpeakerIndex++);
		
		var shouldSkipDialogue = false;

		switch (currentDialogue)
		{
			case GOTOVK:
				GameManager.state.ActiveSpeaker = Character.Player;
				
				shouldSkipDialogue = true;
				break;
			case GOTOBT:
				GameManager.state.ActiveSpeaker = Character.Betaal;
				
				shouldSkipDialogue = true;
				break;
			case GOTOSD:
				GameManager.state.ActiveSpeaker = Character.Oldie;
				
				shouldSkipDialogue = true;
				break;
			case EXIT:
				EndConversation();
				
				if(_isInIntroConv) break;
				CuteCm.only.EndDialogue();
				return true;
		}
		
		if (shouldSkipDialogue)
		{
			currentSpeakerIndex = ref UpdateActiveSpeakerIndex();
			currentDialogue = _currentDialogue.GetDialogue(GameManager.state.ActiveSpeaker, currentSpeakerIndex++);
		}
		
		ShowAppropriateCharacter();
		SpawnDialogSpeaker(GameManager.state.ActiveSpeaker, currentDialogue);
		return false;
	}

	private void ShowAppropriateCharacter()
	{
		switch (GameManager.state.ActiveSpeaker)
		{
			case Character.Player: ShowPlayer();
				break;
			case Character.Betaal: ShowBetaal();
				break;
			case Character.Oldie: ShowOldie();
				break;
			default: throw new ArgumentOutOfRangeException();
		}
	}

	private void ShowPlayer()
	{
		if (_currentLeftCharacter == Character.Player)
		{
			//player name is on the right and image on left
			ShowSpeakerDetails(rightCharacterName, leftImage);
			HideSpeakerDetails(leftCharacterName, rightImage);
		}
		else if(_currentLeftCharacter == Character.Oldie)
		{
			//player name is on the left and image on right
			ShowSpeakerDetails(leftCharacterName, rightImage);
			HideSpeakerDetails(rightCharacterName, leftImage);
		}
	}

	private void ShowBetaal()
	{
		//betaal is always on the right
		ShowSpeakerDetails(leftCharacterName, rightImage);

		//player name is on the right and image on left
		HideSpeakerDetails(rightCharacterName, leftImage);
	}

	private void ShowOldie()
	{
		//oldie is always on left
		ShowSpeakerDetails(rightCharacterName, leftImage);
		
		//player name is on the left and image on right
		HideSpeakerDetails(leftCharacterName, rightImage);
	}

	private ref int UpdateActiveSpeakerIndex()
	{
		ref var currentIdx = ref _currentPlayerIndex;
		switch (GameManager.state.ActiveSpeaker)
		{
			case Character.Player: break;
			case Character.Betaal: currentIdx = ref _currentBetaalIndex;
				break;
			case Character.Oldie: currentIdx = ref _currentSadhuIndex;
				break;
		}

		return ref currentIdx;
	}

	private void EndConversation()
	{
		dialoguePanel.SetActive(false);
		leftPivotPanel.SetActive(false);
		rightPivotPanel.SetActive(false);
		
		GameEvents.InvokeConversationEnd();
		if(_currentConversationIndex++ == 0)
		{
			GameEvents.InvokeIntroConversationComplete();
			if (_currentConversationIndex - 1 == 0) CuteCm.only.SetEndIntroConv();
			_isInIntroConv = false;
		}
		if (_currentConversationIndex > 1 && _currentConversationIndex < dialogueBanks.Count - 1) 
			GameEvents.InvokeBetaalFightStart();
	}

	private void SetSpeakerDetailsPos(Character speaker, out TextMeshProUGUI text, out Image image)
	{
		var convWithBetaal = GameManager.state.InConversationWithBetaal;
		//print(convWithBetaal + ", " + GameManager.state.ActiveSpeaker);
		
		if (speaker == Character.Player)
			if (convWithBetaal)
				SetLeftIcon(out text, out image);
			else
				SetRightIcon(out text, out image);
		else if (speaker == Character.Betaal)
			SetRightIcon(out text, out image);
		else//if (speaker == Character.Oldie) 
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
		if(_tipWaiter.IsActive()) _tipWaiter.Rewind();
		
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

	private void HideSpeakerDetails(TextMeshProUGUI text, Image image)
	{
		image.enabled = false;
		text.enabled = false;
	}

	private void ShowSpeakerDetails(TextMeshProUGUI text, Image image)
	{
		image.enabled = true;
		text.enabled = true;
	}

	private void RestartTipBlinker()
	{
		_tipAppear.Restart();
		_tipBlinker.Restart();
	}


	private void HideTipBlinker() => _tipDisappear.Restart();

	private void OnUsePressed()
	{
		if(!GameManager.state.IsInConversation) return;
		ProgressConversation();
	}

	private void OnConversationStart()
	{
		GameManager.state.ActiveSpeaker = firstWordBy[_currentConversationIndex];
		_currentDialogue = dialogueBanks[_currentConversationIndex];

		GameManager.state.InConversationWithBetaal = GameManager.state.ActiveSpeaker == Character.Betaal;
		
		if (_currentConversationIndex == 0)
		{
			CuteCm.only.SetStartIntroConv();
			_isInIntroConv = true;
		}

		if (GameManager.state.InConversationWithBetaal)
			_currentLeftCharacter = Character.Player;
		else //if (GameManager.state.ActiveSpeaker == Character.Oldie) 
			_currentLeftCharacter = Character.Oldie;

		_currentBetaalIndex = 0;
		_currentPlayerIndex = 0;
		_currentSadhuIndex = 0;
		
		ProgressConversation();
	}
}