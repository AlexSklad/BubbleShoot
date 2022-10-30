using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Game : MonoBehaviour
{
    [SerializeField]
    private uint _scoreForHitBubble = 100;
    [SerializeField]
    private Player _player;
    public enum GameState
    {
        Playing,
        Paused,
        EndGame
    }
    #region GameAction
    public Action OnStateChanged;
    public Action OnVictory;
    public Action OnLoss;
    public Action OnGameStop;
    #endregion
    public uint Score { get; private set; }

    private static Game _instance;

    public static Game Instance
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Game>(true);
            }

            return _instance;
        }
    }

    private GameState _currentState = GameState.EndGame;
    public GameState CurrentState
    {
        get { return _currentState; }
        private set
        {
            _currentState = value;
            OnStateChanged?.Invoke();
        }
    }

    public Player ActivePlayer => _player;

    public LevelHeader CurrentLevel { get; private set; }

    public void StartGame(LevelHeader level)
    {
        CurrentLevel = level;
        LevelManager.OpenLevel(CurrentLevel.LevelName);
        Time.timeScale = 1f;
        Score = 0;
        ActivePlayer.Reset();

        CurrentState = GameState.Playing;
    }

    public void StopGame()
    {
        LevelManager.UnloadLevel();
        Time.timeScale = 1f;
        CurrentState = GameState.EndGame;

        OnGameStop?.Invoke();
    }

    public void Pause()
    {
        if (CurrentState !=GameState.Playing)
        {
            return;
        }
        Time.timeScale = 0f;
        CurrentState = GameState.Paused;
    }

    public void Resume()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    public void Restart()
    {
        StartGame(CurrentLevel);
    }

    public void Win()
    {
        CurrentState = GameState.EndGame;
        OnVictory?.Invoke();
    }

    public void Loss()
    {
        Score += _scoreForHitBubble;
        CurrentState = GameState.EndGame;
        OnLoss?.Invoke();
    }

    public void OnBubbleDestroy()
    {
        Score += _scoreForHitBubble;
        CheckWinCondition();
    }

    public void CheckWinCondition()
    {
        if (BubbleLayout.Instance.BubbleCount == 0)
        {
            Win();
        }

    }

    //public void CheckLossCondition()
    //{
    //    if (_player.LastShotBubble.CollisionLayer == Layer.LastShotBubble && )
    //}
}
