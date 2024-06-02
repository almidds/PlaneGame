// using System.Collections;
// using System.Collections.Generic;

// using UnityEditor;

// using UnityEngine;

// [CustomEditor(typeof(ChunkController))]
// public class ChunkControllerEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         ChunkController cc = (ChunkController)target;
//         if (DrawDefaultInspector())
//         {
//             cc.GenerateChunks();
//         }
//         if (GUILayout.Button("Generate"))
//         {
//             cc.GenerateChunks();
//         }
//     }
// }