using UnityEngine;

public class SocketEditor : MonoBehaviour
{
    [SerializeField] 
    private int _prefabValue;
    [SerializeField] 
    private DragableEditor _dependent;
    [SerializeField] 
    private FurnitureCreator _furnitureCreator;

    public void Fill()
    {
        if (_dependent)
        {
            DestroyImmediate(_dependent);
        }
        DragableEditor prefab = _furnitureCreator.ResourceElements[_prefabValue]._prefab;
        _dependent = Instantiate(prefab);
        _dependent.transform.position = transform.position;
        _dependent.Snap(this, 0);
    }
}
