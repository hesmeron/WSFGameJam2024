using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DranAndDropManager : MonoBehaviour
{
    [SerializeField] 
    private List<DragableBehaviour> _dragables = new List<DragableBehaviour>();
    
    private Camera _mainCamera;
    private Vector3 _planeAnchor;
    private bool _isDragged = false;
    private DragableBehaviour _currentlyDragged;
    private void OnDrawGizmos()
    {

    }
    
    public void Awake()
    {
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
            if (Trigonometry.PointIntersectsAPlane(_mainCamera.transform.position, mousePos, _planeAnchor, Vector3.up, out Vector3 result))
            {
                Vector3 newAnchor = result;
                _planeAnchor = newAnchor;
                _currentlyDragged.Drag(newAnchor);
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
                Vector3 mouseScreenPos = Input.mousePosition;
                Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
                _planeAnchor = hit;
                if (Trigonometry.PointIntersectsAPlane(hit, mousePos, Vector3.zero, Vector3.up, out Vector3 result))
                {

                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
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

        return false;
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
}
