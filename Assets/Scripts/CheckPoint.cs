using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Collider Collider;

    public int Index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Collider = gameObject.GetComponent<Collider>();
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Cart collider"))
            return;
        
        var parent = other.transform.parent.gameObject;

        parent.GetComponent<CartHandle>().PushCheckPoint(this);
        //print($"Colliding {Index}");
    }
}