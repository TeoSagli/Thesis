using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using DilmerGames.Core.Singletons;

public class UIManager : Singleton<UIManager>
{
    private const string GAME_SCENE_NAME = "Game";

    [Header("UI configuration")]
    [SerializeField]
    private float offsetPositionFromPlayer = 1.0f;
    [SerializeField]
    private GameObject menuContainer;

    [Header("Events")]
    public Action onGameResumeActionExecuted;
    private Menu menu;

    private void Awake()
    {
        menu = menuContainer.GetComponentInChildren<Menu>(true); //true if menu is hidden
        menu.resumeButton.onClick.AddListener(() =>
        {
            HandleMenuOptions(GameState.Playing);
            onGameResumeActionExecuted?.Invoke();
        });
        menu.restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(GAME_SCENE_NAME);
            onGameResumeActionExecuted?.Invoke();
        });
    }
    private void OnEnable()
    {
        //listen to game manager state changes
        GameManager.Instance.onGamePaused += HandleMenuOptions;
        GameManager.Instance.onGameResumed += HandleMenuOptions;
        GameManager.Instance.onGameSolved += HandleMenuOptions;
    }
    private void OnDisable()
    {
        //remove listeners
        GameManager.Instance.onGamePaused -= HandleMenuOptions;
        GameManager.Instance.onGameResumed -= HandleMenuOptions;
        GameManager.Instance.onGameSolved -= HandleMenuOptions;
    }
    private void HandleMenuOptions(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Paused:
                ActivateAndShowMenu();
                break;
            case GameState.PuzzleSolved:
                ActivateAndShowMenu();
                menu.resumeButton.gameObject.SetActive(false);
                menu.solvedText.gameObject.SetActive(true);
                break;
            default:
                menuContainer.SetActive(false);
                break;
        }
    }
    private void ActivateAndShowMenu()
    {
        menuContainer.SetActive(true);
        PlaceMenuInFrontOfPlayer();
    }
    private void PlaceMenuInFrontOfPlayer()
    {
        var playerHead = Camera.main.transform;
        menuContainer.transform.position = playerHead.position + (playerHead.forward * offsetPositionFromPlayer);
        menuContainer.transform.rotation = playerHead.rotation;
    }
}
