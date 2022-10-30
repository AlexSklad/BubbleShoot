using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelManager
{
    private static LevelList _levels;

    public static LevelHeader[] Levels
    {
        get
        {
            if (_levels == null)
            {
                _levels = Resources.Load<LevelList>("Levels/LevelList");
            }

            return _levels.Levels;
        }
    }


    public static Level GetLevel(string levelName)
    {
        return Resources.Load<Level>("Levels/" + levelName);
    }

    public static void OpenLevel(string levelName)
    {
        OpenLevel(GetLevel(levelName));
    }

    public static void OpenLevel(Level level)
    {
        UnloadLevel();
        Stage.Instance.SetBackground(level.Background);
        BubbleLayout.Instance.PlaceBubbles(level.GetLayout());
        Game.Instance.ActivePlayer.SetColorSequence(level.UseRandomPlayerColors, level.PlayerColorsSequence);
    }

    public static void UnloadLevel()
    {
        BubbleLayout.Instance.Clear();
    }
}
