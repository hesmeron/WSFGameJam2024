using System;
using Unity.VisualScripting;
using UnityEngine;

public class DragableBehaviour : MonoBehaviour
{
    [SerializeField] 
    private float _radius = 1f;
    [SerializeField] 
    private Vector3 from;
    [SerializeField]
    private Vector3 to;

    private Camera _mainCamera;
    [SerializeField]
    private Vector3 _hookPoint;

    private bool _isDragged = false;

    private void OnDrawGizmos()
    {
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
        }
        int stepCount = 250;
        Gizmos.DrawLine(transform.position + from, transform.position + to);
        Gizmos.color = new Color(0,0,1, 2f/stepCount);

        for (int i = 0; i < stepCount; i++)
        {
            float completion = i / (float)stepCount;
            Vector3 pos = transform.position + Vector3.Lerp(from, to, completion);
            Gizmos.DrawSphere(pos, _radius);
        }

        Vector3 mousePos = GetMouseRelativePos();

        if (DidMouseHit(out Vector3 hit))
        {
            Debug.Log("Hit!");
            Trigonometry.GetCastPoint(From(), To(), hit, out Vector3 closestPos);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(closestPos, hit);
        }
        Gizmos.DrawSphere(mousePos, 0.1f);
        

        //Gizmos.DrawLine(closestPos, mousePos);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(HookPosition(), 0.1f);
        Gizmos.DrawLine(HookPosition(), ClosestMousePoint());
        Gizmos.DrawSphere(Origin(), 0.1f);
/*
        Debug.Log("Vec " + (to - from));
        Vector3 perp = mousePos - Origin();//Trigonometry.Perpendicular(, Vector3.up);
        Debug.Log(perp);
        Gizmos.DrawLine(Origin(), Origin() + perp);
        */

    }

    void Update()
    {
        HandleInput();
        if (_isDragged)
        {
            Vector3 mousePositon = ClosestMousePoint();
            Vector3 translation = mousePositon - HookPosition();
            float magnitude = Mathf.Min(_radius / 2f, translation.magnitude);
            translation = translation.normalized * magnitude;
            if (translation.magnitude > 0.1)
            {
                transform.position = Vector3.Slerp(transform.position, transform.position + translation, 5 * Time.deltaTime);
            }

        }
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (DidMouseHit(out Vector3 hit))
            {
                Debug.Log("Hit!");
                Trigonometry.GetCastPoint(From(), To(), hit, out Vector3 closestPos);
                _isDragged = true;
                _hookPoint = hit - transform.position; //closestPos - transform.position;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
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

    private Vector3 GetMouseRelativePos()
    {        
        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Vector3 direction = Origin() - mousePos;
        return mousePos;
    }
    float CapsuleDistance( Vector3 p, Vector3 a, Vector3 b, float r)
    {
        Vector3 pa = p - a;
        Vector3 ba = b - a;
        float h = Mathf.Clamp( Vector3.Dot(pa,ba)/Vector3.Dot(ba,ba), 0.0f, 1.0f );
        return ( pa - ba*h ).magnitude - r;
    }

    private bool DidMouseHit(out Vector3 hitPoint)
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Vector3 direction = (mousePosition - _mainCamera.transform.position).normalized;

        int rayStep = 100;
        float rayDistance = 100f;
        hitPoint = mousePosition;
        for (int i = 0; i < rayStep; i++)
        {
            float distance = CapsuleDistance(hitPoint, From(), To(), _radius); 
            //Gizmos.color = new Color(1, 0, 0, 0.5f);
            //Gizmos.DrawSphere(rayPos, distance);
            //Gizmos.color = Color.red;
           // Gizmos.DrawSphere(rayPos, 0.1f);
            if (distance <= Single.Epsilon)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawCube(rayPos, Vector3.one * 0.1f);
                return true;
            }
            if(rayDistance > Single.Epsilon)
            {
                float travel = Mathf.Min(distance, rayDistance);
                rayDistance -= travel;
                hitPoint += direction * travel;
            }
            else
            {
                return false;
            }
        }

        return false;
    }   
    
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
            //Gizmos.color = new Color(1, 0, 0, 0.5f);
            //Gizmos.DrawSphere(rayPos, distance);
            //Gizmos.color = Color.red;
           // Gizmos.DrawSphere(rayPos, 0.1f);
            if (distance <= Single.Epsilon)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawCube(rayPos, Vector3.one * 0.1f);
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
}
