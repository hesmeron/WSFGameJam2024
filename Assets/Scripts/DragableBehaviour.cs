using System;
using System.Collections.Generic;
using UnityEngine;

public class DragableBehaviour : MonoBehaviour
{
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
        Gizmos.color = new Color(_isDragged ? 1:0,0, _isDragged ? 0:1, 20f/stepCount);

        for (int i = 0; i < stepCount; i++)
        {
            float completion = i / (float)stepCount;
            Vector3 pos =  Vector3.Lerp(From(), To(), completion);
            Gizmos.DrawSphere(pos, _radius);
        }
    }

    private void Awake()
    {
        foreach (Socket socket in Sockets)
        {
            socket.Init(this);
        }
    }

    public float Distance(Vector3 rayPos)
    {
        return CapsuleDistance(rayPos, From(), To(), _radius);
    }

    public void StartDragging(Vector3 hookPoint)
    {
        _hookPoint = hookPoint - transform.position;
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
    public void SnapTranslate(Vector3 translation, Socket socket)
    {
        _sockets = transform.GetComponentsInChildren<Socket>();
        transform.position += translation;
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
        Vector3 dest = newAnchor - _hookPoint;
        transform.position = Vector3.Slerp(transform.position, dest, 12f * Time.deltaTime);
        foreach (Socket socket in _sockets)
        {
            socket.TransferMomentum();
        }
    }

    private Vector3 From()
    {
        return transform.position + from;
    }  
    
    private Vector3 To()
    {
        return transform.position + to;
    }    
    private Vector3 HookPosition()
    {
        return transform.position + _hookPoint;
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
    private Vector3 ClosestMousePoint()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Vector3 direction = (mousePosition - _mainCamera.transform.position).normalized;

        int rayStep = 100;
        float rayDistance = 100f;
        Vector3 rayPos = mousePosition;
        Vector3 closestPos = mousePosition;
        float minDistance = rayDistance;
        for (int i = 0; i < rayStep; i++)
        {
            float distance = CapsuleDistance(rayPos, From(), To(), _radius);
            if (distance < minDistance)
            {
                closestPos = rayPos;
            }
            if (distance <= Single.Epsilon)
            {
                return closestPos;
            }
            if(rayDistance > Single.Epsilon)
            {
                float travel = Mathf.Min(distance, rayDistance);
                rayDistance -= travel;
                rayPos += direction * travel;
            }
            else
            {
                return closestPos;
            }
        }

        return closestPos;
    }

    private Vector3 CameraDirection()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Vector3 direction = (mousePosition - _mainCamera.transform.position).normalized;
        return direction;
    }
    */
}
