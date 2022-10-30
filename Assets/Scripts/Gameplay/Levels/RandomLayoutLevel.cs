using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level (Random)", order = 1)]
public class RandomLayoutLevel : Level
{
    public override BubblePlacement[] GetLayout()
    {
        return LayoutGenerator.GenerateBubbleLayout();
    }
}
