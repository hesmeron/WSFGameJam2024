using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FurnitureCreator : MonoBehaviour
{
    [SerializeField]
    private string furnitureName = "stolikenvog";
    [SerializeField] 
    private ResourceElement[] resourceElements = Array.Empty<ResourceElement>();
    [SerializeField]    
    private RecipeElement[] recipeElements = Array.Empty<RecipeElement>();

    public ResourceElement[] ResourceElements => resourceElements;

    public void CreateSO()
    {
        for (var index = 0; index < resourceElements.Length; index++)
        {
            resourceElements[index].count = 0;
        }

        DragableEditor[] editors = FindObjectsByType<DragableEditor>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Debug.Log("Editors count " +editors.Length);
        recipeElements = new RecipeElement[editors.Length];
        for (var index = 0; index < editors.Length; index++)
        {
            var editor = editors[index];
            List<int> connections = new List<int>();
            foreach (SocketEditor socketEditor in editor.SocketEditors)
            {
                if (socketEditor.IsOccupied(out int prefabValue))
                {
                    connections.Add(prefabValue);
                }
            }

            resourceElements[editor._prefabId].count++;
            recipeElements[index] = new RecipeElement()
            {
                _elementID = editor._prefabId,
                _connectedIds = connections
            };
        }
        

        FurnitureSO example = ScriptableObject.CreateInstance<FurnitureSO>();
        example.Init(resourceElements, recipeElements, furnitureName);
        string path = $"Assets/ScriptableObjects/{furnitureName}.asset";
        AssetDatabase.CreateAsset(example, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = example;
    }

}
