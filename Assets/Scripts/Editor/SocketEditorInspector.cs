using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SocketEditor))]
public class SocketEditorInspector :Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SocketEditor socektEditor = target as SocketEditor;
        if (GUILayout.Button("Fill"))
        {
            socektEditor.Fill();
        }        
        if (GUILayout.Button("Rotate"))
        {
            socektEditor.Fill();
        }
    }
}
