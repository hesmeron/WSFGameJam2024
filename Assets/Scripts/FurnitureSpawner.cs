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

    void Awake()
    {
        Debug.Log("Spawn!");
        List<DragableBehaviour> dragables = new List<DragableBehaviour>();
        for (int index = 0; index < _furnitureSo._resourceElements.Length; index++)
        {
            var resourceElement = _furnitureSo._resourceElements[index];
            for (int i = 0; i < resourceElement.count; i++)
            {
                dragables.Add(Instantiate(resourceElement._prefab, spawnPoint, Quaternion.identity));
                spawnPoint += (Vector3.forward * 1.2f);
            }
        }
        
        _dragAndDropManager.SetDragables(dragables.ToArray());
    }
}
