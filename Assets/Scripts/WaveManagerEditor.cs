#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaveManager waveManager = (WaveManager)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("UpdateSimulation"))
        {
            waveManager.InitializeCascades();
        }
    }
}
#endif