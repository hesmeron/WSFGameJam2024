using UnityEngine;

public class Socket : MonoBehaviour
{
    [SerializeField] 
    private float _radius = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
    
}
