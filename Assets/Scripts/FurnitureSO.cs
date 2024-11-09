using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeElement
{
    public int _elementID;
    public List<int> _connectedIds;
}    
    
[System.Serializable]
public struct ResourceElement
{
    public DragableBehaviour _prefab;
    public int count;
    public int socketCount;
}

[CreateAssetMenu(fileName = "FurnitureSO", menuName = "Scriptable Objects/FurnitureSO")]
public class FurnitureSO : ScriptableObject
{
    public string Name;
    public  ResourceElement[] _resourceElements = Array.Empty<ResourceElement>();
    public RecipeElement[] _recipeElements = Array.Empty<RecipeElement>();
    
    public void Init( ResourceElement[] resourceElements, RecipeElement[] recipeElements, string name)
    {
        _resourceElements = resourceElements;
        _recipeElements = recipeElements;
        Name = name;
    }
}
