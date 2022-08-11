using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager state;
	[SerializeField] private List<GameObject> carDoors;
	
	public bool betaalFight1Over, betaalFight2Over, startFightAfterNextConversation;
	public bool InConversationWithBetaal { get; set; }
	public bool IsSadhuEvil { get; set; }
	public bool InPreFight { get; private set; }
	public bool IsInConversation { get; private set; }
	public Character ActiveSpeaker { get; set; }

	private readonly Dictionary<GameObject, bool> _carDoorStatuses = new();

	private void OnEnable()
	{
		GameEvents.ConversationStart += OnConversationStart;
		GameEvents.ConversationEnd += OnConversationEnd;
		GameEvents.IntroConversationComplete += OnIntroConversationComplete;
		
		GameEvents.BetaalFightStart += OnCombatBegin;
		GameEvents.BetaalFightEnd += OnCombatEnd;
	}

	private void OnDisable()
	{
		GameEvents.ConversationStart -= OnConversationStart;
		GameEvents.ConversationEnd -= OnConversationEnd;
		GameEvents.IntroConversationComplete -= OnIntroConversationComplete;
		
		GameEvents.BetaalFightStart -= OnCombatBegin;
		GameEvents.BetaalFightEnd -= OnCombatEnd;
	}


	private void Awake()
	{
		bool Singleton()
		{
			if (state)
			{
				Destroy(gameObject);
				return true;
			}
			else
			{
				state = this;
				return false;
			}
		}
		
		if(Singleton()) return;
		
		SceneManager.LoadScene(1, LoadSceneMode.Additive);
		InPreFight = true;
	}

	private void Start() => SetupCarDoorStatuses();

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt)) Time.timeScale = 6f;
		if (Input.GetKeyUp(KeyCode.LeftAlt)) Time.timeScale = 1f;
	}
#endif


	private void SetupCarDoorStatuses()
	{
		foreach (var door in carDoors) _carDoorStatuses.Add(door, door.activeSelf);
	}

	private void EnableAllCarDoors()
	{
		foreach (var door in _carDoorStatuses) door.Key.SetActive(true);
	}

	private void ResetAllDoorsToInit()
	{
		foreach (var door in _carDoorStatuses) door.Key.SetActive(door.Value);
	}

	private void OnConversationStart() => IsInConversation = true;

	private void OnIntroConversationComplete() => InPreFight = false;

	private void OnConversationEnd() => IsInConversation = false;

	public void OnGameplayStart()
	{
		InPreFight = true;
		IsInConversation = true;
	}

	private void OnCombatBegin() => EnableAllCarDoors();
	private void OnCombatEnd(bool isTemporary) => ResetAllDoorsToInit();
}
