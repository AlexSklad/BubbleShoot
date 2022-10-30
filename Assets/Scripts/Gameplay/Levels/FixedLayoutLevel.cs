using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level (Preset)", order = 1)]
public class FixedLayoutLevel : Level
{
    [SerializeField]
    public BubblePlacement[] Bubbles;

    public override BubblePlacement[] GetLayout()
    {
        return Bubbles;
    }
}
