using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelList", menuName = "ScriptableObjects/Level List", order = 1)]
public class LevelList : ScriptableObject
{
    public LevelHeader[] Levels;
}
