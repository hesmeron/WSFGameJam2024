using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Socket))]
public class SocketEditorInspector :Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Socket socket = target as Socket;
        if (GUILayout.Button("Fill"))
        {
            socket.Fill();
        }        
        if (GUILayout.Button("Rotate"))
        {
            socket.Fill();
        }
    }
}
