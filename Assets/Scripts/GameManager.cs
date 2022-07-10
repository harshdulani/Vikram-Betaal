using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager state; 
	public bool InPreFight { get; private set; }
	
	private void OnEnable()
	{
		GameEvents.IntroConversationComplete += OnIntroConversationComplete;
	}

	private void OnDisable()
	{
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
	}

	private void OnIntroConversationComplete() => InPreFight = false;
}
