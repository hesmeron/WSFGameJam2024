using System;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureCreator : MonoBehaviour
{
    [System.Serializable]
    struct RecipeElement
    {
        public int _elementID;
        public List<int> _connectedIds;
    }    
    
    [System.Serializable]
    public struct ResourceElement
    {
        public DragableEditor _prefab;
        public int count;
        public int socketCount;
    }
    
    [SerializeField] 
    private ResourceElement[] resourceElements = Array.Empty<ResourceElement>();
    [SerializeField]    
    private RecipeElement[] recipeElements = Array.Empty<RecipeElement>();

    public ResourceElement[] ResourceElements => resourceElements;

    public void CreateSO()
    {
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
            recipeElements[index] = new RecipeElement()
            {
                _elementID = editor._prefabId,
                _connectedIds = connections
            };
        }
    }
    private void Awake()
    {
        /*List<DragableBehaviour>[] instances = InstantiatePrefabs();
        int[] order = PrepareOrderArray();

        for (var index = 0; index < recipeElements.Length; index++)
        {
            var recipeElement = recipeElements[index];
            DragableBehaviour instance = instances[recipeElement._elementID][order[index]];
            order[index]++;
            for (int t = 0; t < recipeElement._connectedIds.Count; t++)
            {
                int id = recipeElement._connectedIds[t];
                //force join
                foreach (Socket socket in instances[id][order[id]].Sockets)
                {
                    if (!socket.Occupied)
                    {
                        //socket.SnapToSocket();
                        break;
                    }
                }
            }
            recipeElements[index]._connectedIds.Clear();
        }
        */
    }
/*
    private List<DragableBehaviour>[] InstantiatePrefabs()
    {
        List<DragableBehaviour>[] instances = new List<DragableBehaviour>[recipeElements.Length];
        for (int index = 0; index < resourceElements.Length; index++)
        {
            var resourceElement = resourceElements[index];
            for (int i = 0; i < resourceElement.count; i++)
            {
                DragableBehaviour instance = Instantiate(resourceElement._prefab);
                instances[index].Add(instance);
            }
        }

        return instances;
    }
*/
    private int[] PrepareOrderArray()
    {
        int[] order = new int[recipeElements.Length];
        
        for (int o = 0; o < recipeElements.Length; o++)
        {
            order[o] = 0;
        }

        return order;
    }
}
