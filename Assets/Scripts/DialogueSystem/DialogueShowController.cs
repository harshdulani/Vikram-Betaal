using DG.Tweening;
using Player.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueShowController : MonoBehaviour
{
	[Header("Text"), SerializeField] private TextMeshProUGUI leftCharacterName;
	[SerializeField] private TextMeshProUGUI rightCharacterName, dialogue;

	[Header("Image"), SerializeField] private Image leftImage;
	[SerializeField] private Image rightImage;
	[SerializeField] private Sprite vikramIcon, betaalIcon, sadhuIcon, sadhuEvilIcon;
	
	[Header("Tip"), SerializeField] private TextMeshProUGUI tip;
	[SerializeField] private float blinkDuration, blinkWaitTime;

	private DialogueBank _dialogue;
	private int _currentPlayerIndex, _currentBetaalIndex, _currentSadhuIndex;

	private Tween _tipBlinker, _tipWaiter, _tipAppear, _tipDisappear;
	
	private const string GOTOVK = "GOTOVK";
	private const string GOTOBT = "GOTOBT";
	private const string GOTOSD = "GOTOSD";
	private const string EXIT = "EXIT";

	private void OnEnable()
	{
		PlayerInput.UsePressed += OnUsePressed;
	}

	private void OnDisable()
	{
		PlayerInput.UsePressed -= OnUsePressed;
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

	private void SpawnDialog(DialogueBank.Character character, string currentDialogue)
	{
		SetCharacter(DialogueBank.Character.Player);
		SetText(currentDialogue);

		if (_tipBlinker.IsActive())
		{
			_tipBlinker.Pause();
			tip.color = Color.clear;
		}

		if(_tipWaiter.IsActive()) _tipWaiter.Kill();

		_tipWaiter.Restart();
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
		var currentDialogue = _dialogue.GetDialogue(GameManager.state.ActiveSpeaker, _currentPlayerIndex);

		if (currentDialogue.Equals(GOTOVK))
		{
			GameManager.state.ActiveSpeaker = DialogueBank.Character.Player;
		}
		if (currentDialogue.Equals(GOTOBT))
		{
			GameManager.state.ActiveSpeaker = DialogueBank.Character.Betaal;
		}
		if (currentDialogue.Equals(GOTOSD))
		{
			GameManager.state.ActiveSpeaker = DialogueBank.Character.Oldie;
		}
		if (currentDialogue.Equals(EXIT))
		{
			EndConversation();
			return true;
		}
		
		SpawnDialog(GameManager.state.ActiveSpeaker, currentDialogue);
		return false;
	}

	private void EndConversation()
	{
	}

	private void RestartTipBlinker()
	{
		_tipAppear.Restart();
		_tipBlinker.Restart();
	}

	private void HideTipBlinker() => _tipDisappear.Restart();

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

	private void SetText(string text)
	{
		dialogue.text = text;
	}

	private void OnUsePressed()
	{
		if(!GameManager.state.IsInConversation) return;
		if(TryShowNextDialog()) return;
		
		if (_tipBlinker.IsActive())
		{
			_tipBlinker.Pause();
			tip.color = Color.clear;
		}

		HideTipBlinker();
		if(_tipWaiter.IsActive()) _tipWaiter.Kill();
	}
}