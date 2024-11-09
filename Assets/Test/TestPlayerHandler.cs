using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class TestPlayerHandler : MonoBehaviour
{
    public GameObject interactingObject;
    public Transform interactionTransform;
    public TestElement interactionElement;

    public GameObject pointedObject;
    public GameObject highlightedObject;
    public Camera raycastCamera;

    public Transform CameraOrigin;
    public Transform CameraRotation;

    public Transform hitpoint;

    public LayerMask layers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetMouseButton(1))){
            Debug.Log(Input.GetAxis("Horizontal"));
            CameraOrigin.Rotate(0f, 15f * Input.GetAxis("Mouse X"),0f);
            CameraRotation.Rotate( 15f * Input.GetAxis("Mouse Y"),0f, 0f);
        }

        Ray ray = raycastCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.Log("Dupa1");

        Physics.Raycast(ray, out hit, 100f, layers);

        hitpoint.position = hit.point;

        if (interactingObject == null)
        {

            
            if (hit.collider != null)
            {
                Debug.Log("Dupa2");
                pointedObject = hit.collider.gameObject;

                TestSocket socket;

                if (pointedObject.TryGetComponent<TestSocket>(out socket))
                {
                    Debug.Log("Dupa3");
                    highlightedObject = pointedObject;
                    socket.HighlightThis();
                }

                

            }

            if (pointedObject != highlightedObject)
            {
                TestSocketHighlight.sh.UnHighlight();
            }

            if (Input.GetMouseButtonDown(0))
            {
                interactingObject = highlightedObject;
                TestSocketHighlight.sh.UnHighlight();
                TestSocket socket = pointedObject.GetComponent<TestSocket>();
                TestElement element = socket.GetElement();
                interactionElement = element;
                interactionTransform = element.EnterInteraction(socket, hitpoint);
                ChangeLayerRecursively(element.gameObject, LayerMask.NameToLayer("Interacting"));
            }
        }
        else
        {
            if (hit.collider != null)
            {
                Debug.Log("Dupa2");
                pointedObject = hit.collider.gameObject;

                TestSocket socket;

                if (pointedObject.TryGetComponent<TestSocket>(out socket) && interactionTransform.parent != socket.ConnectPoint)
                {
                    Debug.Log("Dupa3");
                    interactionTransform.SetParent(socket.ConnectPoint);
                    interactionTransform.position = socket.ConnectPoint.position;
                    interactionTransform.rotation = socket.ConnectPoint.rotation;
                    interactionTransform.Rotate(0f, 180f, 0f,Space.Self);
                }
                if(pointedObject.TryGetComponent<TestSocket>(out socket) && interactionTransform.parent == socket.ConnectPoint)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        interactionTransform.Rotate(0f, 0f, 90f,Space.Self);
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        interactionTransform.Rotate(0f, 0f, -90f, Space.Self);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        interactionElement.ExitInteraction();

                        ChangeLayerRecursively(interactionElement.gameObject, LayerMask.NameToLayer("Interactable"));
                        interactingObject = null;
                        interactionTransform = null;
                        interactionElement = null;
                        
                    }
                }



            }

            if(pointedObject != hit.collider.gameObject && interactionTransform.parent != hitpoint)
            {
                interactionTransform.SetParent(hitpoint);
            }
        }
    }


    void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitpoint.position, 0.05f);
    }

}
