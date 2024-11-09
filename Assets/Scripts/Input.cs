using UnityEngine;

public static class InputUtility
{
    public static Vector3 WorldMousePosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Camera _mainCamera = Camera.main;
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0.1f));
        if (Trigonometry.PointIntersectsAPlane(_mainCamera.transform.position, mousePos, Vector3.zero, Vector3.up, out Vector3 result))
        {
            return result;
        }

        return Vector3.zero;
    }
}
