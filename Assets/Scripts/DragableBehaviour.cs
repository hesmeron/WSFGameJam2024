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

    private bool _isDragged = false;

    public Socket[] Sockets => _sockets;

    private Vector3 _hookPoint;

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
    }

    private void Awake()
    {
        FillSocketList();
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
        Vector3 dest = newAnchor - (HookPosition() - transform.position) ;
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
}
