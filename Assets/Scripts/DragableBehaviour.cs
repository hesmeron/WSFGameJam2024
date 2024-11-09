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

    private bool _isDragged = false;

    public Socket[] Sockets => _sockets;

    private Vector3 _hookPoint;
    private Vector3 _castPoint;

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
        bool _shouldFall = true;
        foreach (Socket socket in _sockets)
        {
            if (socket.Occupied)
            {
                _shouldFall = false;
                break;
            }
        }
        if (!_isDragged)
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
    }

    public float Distance(Vector3 rayPos)
    {
        return CapsuleDistance(rayPos, From(), To(), _radius);
    }

    public void StartDragging(Vector3 hookPoint)
    {
        _hookPoint = transform.InverseTransformPoint(hookPoint);
        Trigonometry.GetCastPoint(From(), To(), HookPosition(), out Vector3 result);
        _castPoint = transform.InverseTransformPoint(result);
        float distance = Vector3.Distance(Origin(), _castPoint);
        _isDragged = true;
    }
    public void StopDragging()
    {
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
        Debug.Log("Dist " +dist);
        float rotationMagnitude = 12 * Time.deltaTime * dist;
       transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationMagnitude);
       //transform.rotation = t;
        Vector3 dest = newAnchor - (HookPosition() - transform.position);
        float rotationCoeff = 0.5f;

        
        transform.position = Vector3.Slerp(transform.position, dest, 12f * Time.deltaTime);
        foreach (Socket socket in _sockets)
        {
            socket.TransferMomentum();
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
