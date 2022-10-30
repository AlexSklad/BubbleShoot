using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Level : ScriptableObject
{
    public string Background;
    public bool UseRandomPlayerColors;

    [SerializeField]
    public BubbleColor[] PlayerColorsSequence;

    public abstract BubblePlacement[] GetLayout();
}
