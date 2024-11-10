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

    private Rigidbody _rigidbody;
    private bool _isDragged = false;
    private Vector3 _hookPoint;
    private Vector3 _castPoint;
    
    public Socket[] Sockets => _sockets;


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
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //_rigidbody.angularVelocity = _rigidbody.angularVelocity.normalized * Mathf.Min(_rigidbody.angularVelocity.magnitude, 0.1f);
        //_rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * Mathf.Min(_rigidbody.linearVelocity.magnitude, 0.1f);
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
        _rigidbody.useGravity = false;
        _hookPoint = transform.InverseTransformPoint(hookPoint);
        Trigonometry.GetCastPoint(From(), To(), HookPosition(), out Vector3 result);
        _castPoint = transform.InverseTransformPoint(result);
        _isDragged = true;
    }
    public void StopDragging()
    {
        _rigidbody.useGravity = true;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.linearVelocity = Vector3.zero;
        _isDragged = false;
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
        //dist = Vector3.Distance(_castPoint, (from + to) / 2f);
        float rotationMagnitude = 50 * Time.fixedDeltaTime * dist;
       Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationMagnitude);
        Vector3 dest = newAnchor - (HookPosition() - transform.position);
        dest = new Vector3(dest.x, Mathf.Max(dest.y, 6.5f), dest.z);
        
        Vector3 finalDest = Vector3.Slerp(transform.position, dest, 12f * Time.fixedDeltaTime);
        _rigidbody.Move(finalDest, finalRotation);
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
    }
}
