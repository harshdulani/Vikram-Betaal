using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { MainMenu, Playing, Paused }

public class MainMenuController : MonoBehaviour
{
	[SerializeField] private Image black;
	[SerializeField] private GameObject menuPanel, aboutPanel, inGamePanel;
	
	private Tweener _colorTween;
	
	private GameState _currentState;
	private Color _initColor;
	private bool _hasStarted;

	private void Start()
	{
		_initColor = black.color;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
		if (Input.GetKeyUp(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
			default: throw new ArgumentOutOfRangeException();
		}
	}

	private void PauseGame()
	{
		//timescale 0
		inGamePanel.SetActive(true);
		
		if(_colorTween.IsActive()) _colorTween.Kill();
		_colorTween = black.DOColor(_initColor, 0.5f);
	}

	public void OnPressResumeGame()
	{
		//timescale 1
		inGamePanel.SetActive(false);
		
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

	public void OnPressAbout()
	{
		aboutPanel.SetActive(!aboutPanel.activeSelf);
	}

	public void OnPressQuit()
	{
		Application.Quit();
	}
}
