using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager state;
	public bool InConversationWithBetaal { get; set; }
	public bool isSadhuEvil { get; private set; }
	public bool InPreFight { get; private set; }
	public bool IsInConversation { get; private set; }
	public Character ActiveSpeaker { get; set; }
	
	private void OnEnable()
	{
		GameEvents.ConversationStart += OnConversationStart;
		GameEvents.IntroConversationComplete += OnIntroConversationComplete;
	}

	private void OnDisable()
	{
		GameEvents.ConversationStart -= OnConversationStart;
		GameEvents.IntroConversationComplete -= OnIntroConversationComplete;
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

	private void Start()
	{
		InPreFight = true;
		IsInConversation = true;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt)) Time.timeScale = 6f;
		if (Input.GetKeyUp(KeyCode.LeftAlt)) Time.timeScale = 1f;
	}

	private void OnConversationStart()
	{
		IsInConversation = true;
	}

	private void OnIntroConversationComplete()
	{
		InPreFight = false;
		IsInConversation = false;
	}
}
