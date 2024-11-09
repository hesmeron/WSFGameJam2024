using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FurnitureCreator))]
public class FurnitureCreatorEditor :Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        FurnitureCreator creator = target as FurnitureCreator;
        if (GUILayout.Button("Create SO"))
        {
            creator.CreateSO();
        }
    }
}
