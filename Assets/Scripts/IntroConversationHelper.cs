using DG.Tweening;
using UnityEngine;

public class IntroConversationHelper : MonoBehaviour
{
	[SerializeField] private Transform sittingSpot;

	[SerializeField] private DialogueBank.Character character;
	private Animator _anim;
	private static readonly int StandUpSitting = Animator.StringToHash("standUpSitting");

	private void OnEnable()
	{
		GameEvents.IntroConversationComplete += OnIntroConversationComplete;
	}
	
	private void OnDisable()
	{
		GameEvents.IntroConversationComplete -= OnIntroConversationComplete;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();

		sittingSpot.parent = null;
		transform.position = sittingSpot.position;
		transform.rotation = sittingSpot.rotation;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.X)) StandUp();
	}

	private void StandUp()
	{
		_anim.SetTrigger(StandUpSitting);
		
		if(character == DialogueBank.Character.Player)
			transform.DOMoveZ(0f, 1.5f).SetEase(Ease.InQuart)
					 .OnComplete(GameEvents.InvokeIntroConversationComplete);
	}

	private void OnIntroConversationComplete() => StandUp();
}