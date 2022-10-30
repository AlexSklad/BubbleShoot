using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LossScreen : UIPage
{
    [SerializeField]
    private TMPro.TMP_Text _scoreText;

    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    //public void NextLevel()
    //{
    //    Close();

    //    var levels = LevelManager.Levels;
    //    var currentLevelIndex = Array.IndexOf(levels, Game.Instance.CurrentLevel);

    //    Game.Instance.StartGame(levels[currentLevelIndex + 1]);
    //}

    public void Restart()
    {
        Close();

        Game.Instance.Restart();
    }

    public void MainMenu()
    {
        Close();

        Game.Instance.StopGame();
    }

    void OnEnable()
    {
        _scoreText.text = string.Format("{0:n0}", Game.Instance.Score);

        //var levels = LevelManager.Levels;
        //var currentLevelIndex = Array.IndexOf(levels, Game.Instance.CurrentLevel);

        //// hide the next level button if this is the last level
        //_nextLevelButton.SetActive(currentLevelIndex >= 0 && currentLevelIndex < levels.Length - 1);
    }
}
