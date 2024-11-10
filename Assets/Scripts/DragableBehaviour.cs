using System;
using UnityEngine;

public class DragableBehaviour : MonoBehaviour
{
    [SerializeField]
    public int _prefabId;
    [SerializeField]
    private Socket[] _sockets = Array.Empty<Socket>();
    [SerializeField] 
    private float _radius = 1f;
    [SerializeField] 
    private Vector3 from;
    [SerializeField]
    private Vector3 to;
    [SerializeField]
    private float debugDist = 0f;

    [SerializeField] private Rigidbody _rigidbody;

    private bool _isDragged = false;

    public Socket[] Sockets => _sockets;

    private Vector3 _hookPoint;
    private Vector3 _castPoint;
    private bool _becameDependent = false;

    public virtual Rigidbody Rigidbody => _rigidbody;

    private void OnDrawGizmos()
    {
        int stepCount = 250;
        Gizmos.DrawLine(From(), To());
        Gizmos.color = new Color(_isDragged ? 1:0,0, _isDragged ? 0:1, 2f/stepCount);

        for (int i = 0; i < stepCount; i++)
        {
            float completion = i / (float)stepCount;
            Vector3 pos =  Vector3.Lerp(From(), To(), completion);
            Gizmos.DrawSphere(pos, _radius);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(HookPosition(), 0.1f);   
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(CastPoint(), 0.5f);
    }

    private void Awake()
    {
        FillSocketList();
    }

    private void Update()
    {
/*
        if (!_isDragged && _shouldFall)
        {
            Vector3 translation = -Vector3.up * Time.deltaTime;
            Vector3 groundPosition = new Vector3(transform.position.x, 0, transform.position.y);
            float floorHeight = 0f; //_radius / 2f;
            float distance = CapsuleDistance(groundPosition, From(), To(), _radius);//
            distance = (transform.position.y - floorHeight) - Mathf.Max(_radius*2f, Vector3.Distance(From(), To()));
            debugDist = distance;
            if (distance > 0.1f)
            {
                float magnitude = Mathf.Min(translation.magnitude, distance);
                transform.position += translation.normalized * magnitude;
            }
        }
        */
    }

    private void FillSocketList()
    {
        _sockets = transform.GetComponentsInChildren<Socket>();
        foreach (Socket socket in _sockets)
        {
            socket.Init(this);
        }
    }
    public void Init(int index)
    {
        _prefabId = index;
    }

    public void Snap(Socket target, int index = 0)
    {
        FillSocketList();
        Transform socketTransform = _sockets[index].transform;
        Quaternion destSocketRotation = Quaternion.LookRotation(-target.transform.forward, Vector3.up);
        Quaternion destRotation = destSocketRotation * Quaternion.Inverse(socketTransform.localRotation);
        transform.rotation = destRotation;
        Vector3 translation = target.transform.position - socketTransform.position;
        transform.position += translation;
        DisablePhysiss(target);
    }
    
    public void Snap(Socket target, Socket own)
    {
        FillSocketList();
        Transform socketTransform = own.transform;
        Quaternion destSocketRotation = Quaternion.LookRotation(-target.transform.forward, Vector3.up);
        Quaternion destRotation = destSocketRotation * Quaternion.Inverse(socketTransform.localRotation);
        transform.rotation = destRotation;
        Vector3 translation = target.transform.position - socketTransform.position;
        transform.position += translation;
        DisablePhysiss(target);
    }

    private void DisablePhysiss(Socket target)
    {
        transform.SetParent(target.transform.parent);
        _becameDependent = true;
        Destroy(_rigidbody);
        _rigidbody = target.DragableBehaviour.Rigidbody;
    }

    public float Distance(Vector3 rayPos)
    {
        return CapsuleDistance(rayPos, From(), To(), _radius);
    }

    public void StartDragging(Vector3 hookPoint)
    {
       // if (!_becameDependent)
       // {
            _rigidbody.useGravity = false;
        //}
        _hookPoint = transform.InverseTransformPoint(hookPoint);
        Trigonometry.GetCastPoint(From(), To(), HookPosition(), out Vector3 result);
        _castPoint = transform.InverseTransformPoint(result);
        _isDragged = true;
    }
    public void StopDragging()
    {
       // if (!_becameDependent)
        //{
            _rigidbody.useGravity = true;
       // }
        _isDragged = false;
    }

    public void SnapTranslate(Vector3 translation)
    {
        transform.position += translation;
    }   
    public void Snap(Vector3 translation, Quaternion rotation, Socket socket)
    {
        _sockets = transform.GetComponentsInChildren<Socket>();
        transform.position += translation;
        transform.rotation = rotation;
        foreach (Socket otherSocket in _sockets)
        {
            if (otherSocket != socket)
            {
                otherSocket.TransferMomentum();
            }
        }
    }

    public void Drag(Vector3 newAnchor)
    {
        Quaternion fromToRot = Quaternion.FromToRotation(HookPosition() - Origin(), newAnchor - Origin());
        Quaternion targetRotation = fromToRot * transform.rotation;
        float dist = Mathf.Clamp(Vector3.Distance(_castPoint, (from + to)/2f) -0.5f, 0, 1);
        dist = Vector3.Distance(_castPoint, (from + to) / 2f);
        float rotationMagnitude = 12 * Time.deltaTime * dist;
       Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationMagnitude);
        Vector3 dest = newAnchor - (HookPosition() - transform.position);
        
        Vector3 finalDest = Vector3.Slerp(transform.position, dest, 12f * Time.deltaTime);
        if (_becameDependent)
        {
            
            _rigidbody.MovePosition(finalDest);
        }
        else
        {
            _rigidbody.Move(finalDest, finalRotation);
        }

    }

    private Vector3 From()
    {
        return transform.TransformPoint(from);
    }  
    
    private Vector3 To()
    {
        return transform.TransformPoint(to);
    }    
    private Vector3 HookPosition()
    {
        return  transform.TransformPoint(_hookPoint);
    }    
    
    private Vector3 CastPoint()
    {
        return  transform.TransformPoint(_castPoint);
    }

    private Vector3 Origin()
    {
        return (From() + To()) / 2;
    }
    
    float CapsuleDistance( Vector3 p, Vector3 a, Vector3 b, float r)
    {
        Vector3 pa = p - a;
        Vector3 ba = b - a;
        float h = Mathf.Clamp( Vector3.Dot(pa,ba)/Vector3.Dot(ba,ba), 0.0f, 1.0f );
        return ( pa - ba*h ).magnitude - r;
        
        /*
        int stepCount = 250;

        float minDistance = 100f;
        for (int i = 0; i < stepCount; i++)
        {
            float completion = i / (float)stepCount;
            Vector3 pos = transform.position + Vector3.Lerp(from, to, completion);
            minDistance = Mathf.Min(minDistance, Vector3.Distance(p, pos));
        }

        return minDistance - r;
        */
    }

    private bool IsFree()
    {
        bool _shouldFall = true;
        foreach (Socket socket in _sockets)
        {
            if (socket.Occupied)
            {
                _shouldFall = false;
                break;
            }
        }

        return _shouldFall;
    }
    
    float CapsuleDistance( Vector3 p, float h, float r )
    {
        p.y -= Mathf.Clamp( p.y, 0.0f, h );
        return p.magnitude - r;
    }
/*
    float CubeDistance(Vector3 point, Vector3 center, Vector3 extent)
    {
        float xd = Mathf.Abs(point.x - center.x) - extent.x;
        float yd = Mathf.Abs(point.y - center.y) - extent.y;
        float zd = Mathf.Abs(point.z - center.z) - extent.z;
        return 
    }
    */
    float sdBox( Vector3 point, Vector3 center, Vector3 extent )
    {
        //vec3 q = abs(p) - b;
        //return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
        return 0;               
    }
}
