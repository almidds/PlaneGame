using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(Chunk))]
public class ChunkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Chunk chunk = (Chunk)target;
        if (DrawDefaultInspector())
        {
            if (chunk.autoUpdate) {
                chunk.GenerateMap();
            }
        }
        if (GUILayout.Button("Generate"))
        {
            chunk.GenerateMap();
        }
    }
}