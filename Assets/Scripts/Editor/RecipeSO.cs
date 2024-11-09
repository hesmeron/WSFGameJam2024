using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class RecipeSO : ScriptableObject
{
    struct RecipeElement
    {
        public int _elementID;
        public int[] _connectedIds;
    }
    [SerializeField] 
    private DragableBehaviour[] _furnitureElements = Array.Empty<DragableBehaviour>();
}
