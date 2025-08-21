using UnityEngine;

public class ControlRotating : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserControl _userControl;
    void Start()
    {
       _userControl = gameObject.GetComponentInParent<UserControl>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.AngleAxis(_userControl.Angle, transform.up);
    }
}
