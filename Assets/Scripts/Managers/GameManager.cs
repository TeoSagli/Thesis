using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [field: SerializeField]
    private GameState gameState { get; set; } = GameState.Playing;

    [Header("Events")]
    public Action<GameState> onGameResumed;
    public Action<GameState> onGamePaused;
    public Action<GameState> onGameSolved;
    private LayerMask cachedCameraCullingMask;
    private void Awake()
    {
        cachedCameraCullingMask = Camera.main.cullingMask;
    }
    private void OnEnable()
    {
        ControllerManager.Instance.onControllerMenuActionExecuted += ToggleGameState;
        UIManager.Instance.onGameResumeActionExecuted += ToggleGameState;
        PuzzleSolverFeature.Instance.onPuzzleSolved += GameSolved;
    }
    private void OnDisable()
    {
        ControllerManager.Instance.onControllerMenuActionExecuted -= ToggleGameState;
        UIManager.Instance.onGameResumeActionExecuted -= ToggleGameState;
        PuzzleSolverFeature.Instance.onPuzzleSolved -= GameSolved;
    }
    private void GameSolved(GameState gameState)
    {
        this.gameState = gameState;
        CommitGameStateChanges();
    }
    private void ToggleGameState()
    {
        gameState = gameState == GameState.Playing ? GameState.Paused : GameState.Playing;
        CommitGameStateChanges();
    }
    private void CommitGameStateChanges()
    {
        switch (gameState)
        {
            case GameState.Paused:
                InvokeActionSetTimeAndCullingMask(GameState.Paused, ref onGamePaused, 0, LayerMask.GetMask("UI"));
                break;
            case GameState.PuzzleSolved:
                InvokeActionSetTimeAndCullingMask(GameState.PuzzleSolved, ref onGameSolved, 0, LayerMask.GetMask("UI"));
                break;
            default:
                InvokeActionSetTimeAndCullingMask(GameState.Playing, ref onGameResumed, 1, cachedCameraCullingMask);
                break;
        }
    }
    private void InvokeActionSetTimeAndCullingMask(GameState state, ref Action<GameState> action, int time, LayerMask layer)
    {
        action?.Invoke(state);
        Time.timeScale = time; //game time scale which passes
        Camera.main.cullingMask = layer;
    }


}
