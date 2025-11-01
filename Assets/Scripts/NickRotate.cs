using TMPro;
using Unity.Collections;
using UnityEngine;

//todo
public class NickRotate : MonoBehaviour
{
    public GameObject Camera;
    private RectTransform rectTransform;
    public CartHandle CartHandle;
    public bool NickSetup = false;
    public void Start()
    {
        Camera = GameObject.Find("Camera");
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        var angle = Quaternion.LookRotation(transform.position - Camera.transform.position);
        rectTransform.rotation = angle;

        if (!NickSetup)
        {
            if (!string.IsNullOrEmpty(CartHandle.Nickname.Value.Value))
            {
                NickSetup = true;
                GetComponent<TMP_Text>().text = CartHandle.Nickname.Value.Value;
            }
        }
    }
}
