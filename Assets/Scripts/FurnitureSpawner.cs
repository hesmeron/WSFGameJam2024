using System.Collections.Generic;
using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    [SerializeField] 
    private DragAndDropManager _dragAndDropManager;
    [SerializeField] 
    private FurnitureSO _furnitureSo;
    [SerializeField] 
    private Vector3 spawnPoint;
    [SerializeField] 
    private int points = 0;
    [SerializeField]
    int maxPoints = 0;

    private List<DragableBehaviour> dragables;

    void Awake()
    {
        Debug.Log("Spawn!");
        dragables = new List<DragableBehaviour>();
        for (int index = 0; index < _furnitureSo._resourceElements.Length; index++)
        {
            var resourceElement = _furnitureSo._resourceElements[index];
            for (int i = 0; i < resourceElement.count; i++)
            {
                var instance = Instantiate(resourceElement._prefab, spawnPoint, Quaternion.identity);
                instance.Init(index);
                dragables.Add(instance);
                spawnPoint += (Vector3.forward * 1.2f);
            }
        }
        
        _dragAndDropManager.SetDragables(dragables.ToArray());
    }

    public void Judge()
    {
        var recipeElements = _furnitureSo._recipeElements;
        maxPoints = 0;

        foreach (var element in recipeElements)
        {
            maxPoints += element._connectedIds.Count;
        }
        
        foreach (var dragable in dragables)
        {
            foreach (Socket socket in dragable.Sockets)
            {
                if (socket.Occupied)
                {
                    int index = socket.DragableBehaviour._prefabId;
                    for (int i = 0; i < recipeElements.Length; i++)
                    {
                        var element = recipeElements[i];
                        int otherId = socket.JoinedSocket.DragableBehaviour._prefabId;
                        if (element._elementID == index && element._connectedIds.Contains(otherId))
                        {
                            recipeElements[i]._connectedIds.Remove(otherId);
                            points++;
                        }
                    }
                }
            }
        }
    }
}
