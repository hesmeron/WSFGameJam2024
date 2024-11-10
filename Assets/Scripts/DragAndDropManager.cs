using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DragAndDropManager : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField] 
    private DragableBehaviour[] _dragables = Array.Empty<DragableBehaviour>();
    
    private Camera _mainCamera;
    private Vector3 _planeAnchor;
    [SerializeField]
    private bool _isDragged = false;
    [SerializeField]
    private DragableBehaviour _currentlyDragged;

    private void OnDrawGizmos()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Gizmos.color = Color.white;
        Gizmos.DrawCube(_planeAnchor, Vector3.one * 0.2f);
        Gizmos.DrawLine(_planeAnchor, mousePos);
        Vector3 dir = _planeAnchor - mousePos;
        Vector3 a = -Trigonometry.Perpendicular(dir, Vector3.up);
        Vector3 b = -Trigonometry.Perpendicular(dir, Vector3.right);
        Vector3 c = -Trigonometry.Perpendicular(dir, Vector3.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_planeAnchor, _planeAnchor + a);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_planeAnchor, _planeAnchor + b);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_planeAnchor, _planeAnchor + c);
    }

    public void SetDragables(DragableBehaviour[] dragables)
    {
        _dragables = dragables;
    }
    
    public void Awake()
    {
        _dragables = FindObjectsByType<DragableBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (!_mainCamera)
        {
            _mainCamera = Camera.main;
        }
    }
    
    void Update()
    {
        HandleInput();

        if (_isDragged)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));

            Vector3 dir = _planeAnchor - mousePos;
            Vector3 a = -Trigonometry.Perpendicular(dir, Vector3.up);
            Vector3 b = -Trigonometry.Perpendicular(dir, Vector3.right);
            Vector3 c = -Trigonometry.Perpendicular(dir, Vector3.forward);

            Trigonometry.PointIntersectsAPlane(_mainCamera.transform.position, mousePos, _planeAnchor, Vector3.up,
                out Vector3 resultZX);
            Trigonometry.PointIntersectsAPlane(_mainCamera.transform.position, mousePos, _planeAnchor, Vector3.right,
                out Vector3 resultZY);
            Trigonometry.PointIntersectsAPlane(_mainCamera.transform.position, mousePos, _planeAnchor, Vector3.forward,
                out Vector3 resultXY);
            float distZX = Vector3.Distance(_planeAnchor, resultZX);
            float distZY = Vector3.Distance(_planeAnchor, resultXY);
            float distXY = Vector3.Distance(_planeAnchor, resultXY);
            if (distZX < distZY)
            {
                if(distZX < distXY)
                {
                    DragTarget(resultZX);
                }
                else
                {
                    DragTarget(resultXY);
                }
            }
            else
            {
                if(distZY < distXY)
                {
                    DragTarget(resultZY);
                }
                else
                {
                    DragTarget(resultXY);
                }
            }
            HandleSockets();
        }
        

    }
    
    private void DragTarget(Vector3 anchor)
    {
        Vector3 newAnchor = anchor;
        _planeAnchor = newAnchor;
        _currentlyDragged.Drag(newAnchor);
    }

    private void HandleSockets()
    {
        foreach (Socket draggedSocket  in _currentlyDragged.Sockets)
        {
            if (!draggedSocket.Occupied)
            {
                foreach (DragableBehaviour dragableBehaviour in _dragables)
                {
                    if (dragableBehaviour == _currentlyDragged)
                    {
                        continue;
                    }

                    foreach (Socket otherSocket in dragableBehaviour.Sockets)
                    {
                        if (otherSocket.IsInRange(draggedSocket))
                        {
                            //join together
                            Socket.JoinSockets(draggedSocket, otherSocket);
                            StopDragging();
                            break;
                        }
                    }
                }
            }
        }
    }
    
    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (DidMouseHit(out Vector3 hit, out DragableBehaviour closestDragableBehaviour))
            {
                Debug.Log("Hit!");
                _isDragged = true;
                _currentlyDragged = closestDragableBehaviour;
                closestDragableBehaviour.StartDragging(hit); 
                //Vector3 mouseScreenPos = Input.mousePosition;
                //Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
                _planeAnchor = hit;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }
    }
    
    private bool DidMouseHit(out Vector3 hitPoint, out DragableBehaviour closestDragableBehaviour)
    {
        closestDragableBehaviour = null;
        Vector3 mouseScreenPos = Input.mousePosition;

        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        Vector3 direction = (mousePosition - _mainCamera.transform.position).normalized;

        int rayStep = 100;
        float rayDistance = 100f;
        hitPoint = mousePosition;
        for (int i = 0; i < rayStep; i++)
        {
            float distance = MinDistance(hitPoint, out closestDragableBehaviour);
            if (distance <= Single.Epsilon)
            {
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

        return true;
    }

    private float MinDistance(Vector3 point, out DragableBehaviour closestDragableBehaviour)
    {
        float minDistance = 100f;
        closestDragableBehaviour = null;
        foreach (DragableBehaviour dragableBehaviour in _dragables)
        {
            float instanceDistance = dragableBehaviour.Distance(point);
            if (instanceDistance < minDistance)
            {
                closestDragableBehaviour = dragableBehaviour;
                minDistance = instanceDistance;
            }
        }

        return minDistance;
    }

    private void StopDragging()
    {
        if (_isDragged)
        {
            _isDragged = false;
            _currentlyDragged.StopDragging();
        }
    }
}
