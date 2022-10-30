using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FixedLayoutLevel))]
public class FixedLevelInspector : Editor
{
    private FixedLayoutLevel _target;

    private void OnEnable()
    {
        _target = (FixedLayoutLevel)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate random layout"))
        {
            _target.Bubbles = LayoutGenerator.GenerateBubbleLayout();

            EditorUtility.SetDirty(_target);
        }
    }
}