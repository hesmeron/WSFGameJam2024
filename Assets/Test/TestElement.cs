using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TestElement : MonoBehaviour
{
    public Transform InteractionPoint;
    public List<TestSocket> sockets = new List<TestSocket>();

    public GameObject Highlight;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in this.transform)
        {
            TestSocket tmpS;
            
            if(child.gameObject.TryGetComponent<TestSocket>(out tmpS)){
                sockets.Add(tmpS);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform EnterInteraction(TestSocket socket,Transform hitpoint)
    {
        InteractionPoint.position = socket.ConnectPoint.position;
        InteractionPoint.rotation = socket.ConnectPoint.rotation;
        InteractionPoint.transform.SetParent(hitpoint);
        this.transform.SetParent(InteractionPoint.transform);

        return InteractionPoint.transform;
    }

    public void ExitInteraction()
    {

    }


    public void HighLightThis()
    {
        if (Highlight)
        {
            Highlight.gameObject.SetActive(true);
        }
    }
    public void UnhighlightThis()
    {
        if (Highlight)
        {
            Highlight.gameObject.SetActive(false);
        }
    }
}
