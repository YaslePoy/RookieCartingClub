using UnityEngine;

public class NickRotate : MonoBehaviour
{
    public GameObject Camera;
    private RectTransform rectTransform;
    public void Start()
    {
        Camera = GameObject.Find("Camera");
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        var angle = Quaternion.LookRotation(transform.position - Camera.transform.position);
        print(angle.eulerAngles);
        rectTransform.rotation = angle;
    }
}
