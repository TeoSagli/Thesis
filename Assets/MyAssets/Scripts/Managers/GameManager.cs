using System;
using System.Collections;
using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using static Enums;

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
    /// <summary>
    /// Subscribe the other managers to the actions to perform
    /// </summary>
    private void OnEnable()
    {
        ControllerManager.Instance.onControllerMenuActionExecuted += ToggleGameState;
        UIManager.Instance.onGameResumeActionExecuted += ToggleGameState;
        PuzzleManager.Instance.onPuzzleSolved += GameSolved;
    }
    /// <summary>
    /// Unsubscribe the other managers to the actions to perform
    /// </summary>
    private void OnDisable()
    {
        ControllerManager.Instance.onControllerMenuActionExecuted -= ToggleGameState;
        UIManager.Instance.onGameResumeActionExecuted -= ToggleGameState;
        PuzzleManager.Instance.onPuzzleSolved -= GameSolved;
    }
    /// <summary>
    /// Execute after game is solved
    /// </summary>
    private void GameSolved(GameState gameState)
    {
        this.gameState = gameState;
        CommitGameStateChanges();
    }
    /// <summary>
    /// Toggle game state between paused and playing
    /// </summary>
    private void ToggleGameState()
    {
        gameState = gameState == GameState.Playing ? GameState.Paused : GameState.Playing;
        CommitGameStateChanges();
    }
    /// <summary>
    /// Commit changes by current game state
    /// </summary>
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
    /// <summary>
    /// Define the action, time and layer by each different game state
    /// </summary>
    /// <param name="state">Current game state.</param>
    /// <param name="action">Action to execute.</param>
    /// <param name="time">Current time.</param>
    /// <param name="layer">Current layer to interact with.</param>
    /// 
    private void InvokeActionSetTimeAndCullingMask(GameState state, ref Action<GameState> action, int time, LayerMask layer)
    {
        action?.Invoke(state);
        Time.timeScale = time; //game time scale which passes
        Camera.main.cullingMask = layer;
    }
    /// <summary>
    /// Return the current game state.
    /// </summary>
    /// 
    public GameState GetGameState()
    {
        return gameState;
    }


}
