using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField] 
    private int _prefabValue;
#if UNITY_EDITOR
    [SerializeField] 
    private FurnitureCreator _furnitureCreator;
#endif
    [SerializeField] 
    private float _radius = 1f;
    [SerializeField] 
    private DragableBehaviour _dependent;
    private Socket _joinedSocket;
    private DragableBehaviour _dragableBehaviour;
    private bool _occupied = false;

    private FixedJoint _joint;
    public DragableBehaviour DragableBehaviour => _dragableBehaviour;
    public Socket JoinedSocket => _joinedSocket;

    public bool IsOccupied(out int prefabValue)
    {
        prefabValue = _prefabValue;
        return _dependent != null;
    }

    public bool Occupied => _occupied;
#if UNITY_EDITOR
    public void Fill()
    {
        if (_dependent)
        {
            DestroyImmediate(_dependent);
        }

        if (_furnitureCreator == null)
        {
            _furnitureCreator = FindFirstObjectByType<FurnitureCreator>();
        }
        DragableBehaviour prefab = _furnitureCreator.ResourceElements[_prefabValue]._prefab;
        prefab.Init(_prefabValue);
        _dependent = Instantiate(prefab);
        _dependent.transform.position = transform.position;
        _dependent.Snap(this, 0);
    }
    #endif

    private void OnDrawGizmos()
    {
        if (!_occupied)
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
            Gizmos.DrawLine(Pivot(), Pivot() + transform.forward);
        }
    }

    public void Init(DragableBehaviour dragableBehaviour)
    {
        _dragableBehaviour = dragableBehaviour;
    }

    public Vector3 Pivot()
    {
        return transform.position;
    }
    
    public bool IsInRange(Socket socket)
    {
        if (_occupied)
        {
            return false;
        }

        if (Vector3.Angle(transform.forward, -socket.transform.forward) < 60)
        {
            return Vector3.Distance(socket.Pivot(), Pivot()) <= _radius + socket._radius;
        }

        return false;
    }

    public void JoinSocket(Socket socket)
    {
        _joint = transform.parent.gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = socket.DragableBehaviour.Rigidbody;
        _occupied = true;
        _joinedSocket = socket;
    }   
    
    public void SnapToSocket(Socket socket)
    {
        Vector3 translation = socket.Pivot() - Pivot();
        Quaternion destSocketRotation = Quaternion.LookRotation(-socket.transform.forward, Vector3.up);
        Quaternion destRotation = destSocketRotation * Quaternion.Inverse(transform.localRotation);
        _dragableBehaviour.Snap(translation, destRotation, this);
        if (!_occupied)
        {
            JoinSocket(socket);
        }
    }    

    public void TransferMomentum()
    {
        if (_occupied)
        {
            _joinedSocket.SnapToSocket(this);
        }
    }

    public void Snap(Socket other)
    {
        _dragableBehaviour.Snap(other, this);
        JoinSocket(other);
    }

    public static void JoinSockets(Socket adjustable, Socket b)
    {
        Debug.Log("Snap scoket");
        adjustable.Snap(b);
        b.JoinSocket(adjustable);
    }
}
