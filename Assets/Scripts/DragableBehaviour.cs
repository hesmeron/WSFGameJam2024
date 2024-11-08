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
        int stepCount = 25;
        Gizmos.DrawLine(transform.position + from, transform.position + to);
        Gizmos.color = new Color(0,0,1, 2f/stepCount);

        for (int i = 0; i < stepCount; i++)
        {
            float completion = i / (float)stepCount;
            Vector3 pos = transform.position + Vector3.Lerp(from, to, completion);
            Gizmos.DrawSphere(pos, _radius);
        }

        Vector3 mousePos = GetMouseRelativePos();

        Gizmos.DrawSphere(mousePos, 0.1f);
        
        Trigonometry.GetCastPoint(From(), To(), mousePos, out Vector3 closestPos);
        if (Vector3.Distance(closestPos, mousePos) <= _radius)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawLine(closestPos, mousePos);
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(HookPosition(), 0.1f);
        Gizmos.DrawSphere(Origin(), 0.1f);

        Vector3 perp = Trigonometry.Perpendicular(to - from, Vector3.up);
        Gizmos.DrawLine(Origin(), Origin() + );
    }

    void Update()
    {
        HandleInput();
        if (_isDragged)
        {
            Vector3 mousePositon = GetMouseRelativePos();
            Vector3 translation = mousePositon - HookPosition();
            transform.position = Vector3.Slerp(transform.position, transform.position + translation, 10 * Time.deltaTime);
        }
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseRelativePos();
            Trigonometry.GetCastPoint(From(), To(), mousePos, out Vector3 closestPos);
            if (Vector3.Distance(closestPos, mousePos) <= _radius)
            {
                _isDragged = true;
                _hookPoint = closestPos - transform.position;
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

    /*
    private Vector3 Normal()
    {
        return Vector3.pTo() - From()
    }
    */

    private Vector3 GetMouseRelativePos()
    {        
        Vector3 mouseScreenPos = Input.mousePosition;
        float distance = transform.position.z - _mainCamera.transform.position.z;
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        return mousePos;
    }
}
