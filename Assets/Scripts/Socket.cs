using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField] 
    private Vector3 _normal;
    [SerializeField] 
    private float _radius = 1f;

    private Socket _joinedSocket;
    private DragableBehaviour _dragableBehaviour;
    private bool _occupied = false;

    public bool Occupied => _occupied;
    public Vector3 Normal => _normal;

    private void OnDrawGizmos()
    {
        if (!_occupied)
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
            Gizmos.DrawLine(Pivot(), Pivot() + _normal);
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
        if (_occupied || Vector3.Angle(_normal, socket._normal) < 89)
        {
            return false;
        }
        return Vector3.Distance(socket.Pivot(), Pivot()) <= _radius + socket._radius;
    }

    public void JoinSocket(Socket socket)
    {
        _occupied = true;
        _joinedSocket = socket;
    }   
    
    public void SnapToSocket(Socket socket)
    {
        Vector3 translation = socket.Pivot() - Pivot();
        _dragableBehaviour.SnapTranslate(translation, this);
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

    public static void JoinSockets(Socket adjustable, Socket b)
    {
        Debug.Log("Snap scoket");
        adjustable.SnapToSocket(b);
        b.JoinSocket(adjustable);
    }
}
