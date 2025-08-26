using UnityEngine;

public class BreaksHandle : MonoBehaviour
{
    public PlaneResistant BreakResistant;
    private UserControl _control;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _control = GetComponentInParent<UserControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BreakResistant.K = _control.Breaks;
    }
}
