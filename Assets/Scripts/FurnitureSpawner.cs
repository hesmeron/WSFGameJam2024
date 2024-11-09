using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    [SerializeField] 
    private FurnitureSO _furnitureSo;
    [SerializeField] 
    private Vector3 spawnPoint;

    void Awake()
    {
        Debug.Log("Spawn!");
        foreach (ResourceElement resourceElement in _furnitureSo._resourceElements)
        {
            for (int i = 0; i < resourceElement.count; i++)
            {
                Instantiate(resourceElement._prefab);
            }
        }
    }
}
