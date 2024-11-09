using System;
using Unity.Mathematics;
using UnityEngine;

public class DragableEditor : MonoBehaviour
{
    [SerializeField] 
    private SocketEditor[] _socketEditors;

    private void Awake()
    {
        _socketEditors = transform.GetComponentsInChildren<SocketEditor>();
    }

    public void Snap(SocketEditor target, int index = 0)
    {
        Transform socketTransform = _socketEditors[index].transform;
        Quaternion destSocketRotation = quaternion.LookRotation(-target.transform.forward, Vector3.up);
        Quaternion destRotation = destSocketRotation * Quaternion.Inverse(socketTransform.localRotation);
        transform.rotation = destRotation; //Quaternion.LookRotation(-target.transform.right);
        Vector3 translation = target.transform.position - socketTransform.position;
        transform.position += translation;
    }
}
