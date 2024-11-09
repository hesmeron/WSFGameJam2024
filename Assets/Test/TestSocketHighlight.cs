using UnityEngine;

public class TestSocketHighlight : MonoBehaviour
{
    public static TestSocketHighlight sh;


    private void Awake()
    {
        sh = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void HighLight(Transform Hobject)
    {
        sh.transform.position = Hobject.position;
        sh.transform.rotation = Hobject.rotation;
    }

    public void UnHighlight()
    {
        sh.transform.position = new Vector3 (0, -10, 0);
    }
}
