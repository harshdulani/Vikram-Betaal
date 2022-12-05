using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { MainMenu, Playing, Paused, Ended }

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private Image black;
	[SerializeField] private GameObject menuPanel, aboutPanel, inGamePanel, retryPanel, winPanel;
	
	private Tweener _colorTween;
	
	private GameState _currentState;
	private Color _initColor;
	private bool _hasStarted;

	private void OnEnable()
	{
		GameEvents.GameLose += OnGameLose;
		GameEvents.GameWin += OnGameWin;
	}

	private void OnDisable()
	{
		GameEvents.GameLose -= OnGameLose;
		GameEvents.GameWin -= OnGameWin;
	}

	private void Start()
	{
		_initColor = black.color;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
	#if UNITY_EDITOR
		if (Input.GetKeyUp(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	#endif
	}

	private void HandleEscape()
	{
		switch (_currentState)
		{
			case GameState.MainMenu: return;
			case GameState.Playing:
				PauseGame();
				break;
			case GameState.Paused:
				OnPressResumeGame();
				break;
			case GameState.Ended:
				return;
			default: throw new ArgumentOutOfRangeException();
		}
	}

	private void PauseGame()
	{
		Time.timeScale = 0f;
		inGamePanel.SetActive(true);
		_currentState = GameState.Paused;
		
		if(_colorTween.IsActive()) _colorTween.Kill();
		_colorTween = black.DOColor(_initColor, 0.5f);
	}

	private void ShowRetryPanel()
	{
		retryPanel.SetActive(true);
	}

	private void ShowWinPanel()
	{
		winPanel.SetActive(true);
	}
	
	public void OnPressResumeGame()
	{
		Time.timeScale = 1f;
		inGamePanel.SetActive(false);
		_currentState = GameState.Playing;
		
		if(_colorTween.IsActive()) _colorTween.Kill();
		_colorTween = black.DOColor(Color.clear, 0.5f);
	}

	public void OnPressNewGame()
	{
		if(_hasStarted) return;

		_hasStarted = true;
		menuPanel.SetActive(false);
		aboutPanel.SetActive(false);
		_currentState = GameState.Playing;
		
		GameEvents.InvokeGameplayStart();
		if(_colorTween.IsActive()) _colorTween.Kill();
		_colorTween = black.DOColor(Color.clear, 0.5f);
	}

	public void OnPressRetry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnPressAbout()
	{
		aboutPanel.SetActive(!aboutPanel.activeSelf);
	}

	public void OnPressQuit()
	{
		Application.Quit();
	}

	private void OnGameLose()
	{
		_currentState = GameState.Ended;
		DOVirtual.DelayedCall(2f, ShowRetryPanel);
	}

	private void OnGameWin()
	{
		_currentState = GameState.Ended;
		DOVirtual.DelayedCall(2f, ShowWinPanel);
	}
}
