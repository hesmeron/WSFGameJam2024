using UnityEngine;

public class TestSocket : MonoBehaviour
{
    public Transform ConnectPoint;
    public Transform HighLightPoint;

    Transform parentGO;
    public TestElement parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        parentGO = transform.parent;
        parent = parentGO.gameObject.GetComponent<TestElement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Transform GetConnetPoint()
    {
        return ConnectPoint;
    }

    public void HighlightThis()
    {
        TestSocketHighlight.sh.HighLight(HighLightPoint);
    }

    public TestElement GetElement()
    {
        return parent;
    }
}
