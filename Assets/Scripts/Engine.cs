using UnityEngine;

public class Engine : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private UserControl _control;
    public AnimationCurve AnimationCurve;
    protected float _currentForce;
    public float CurrentForce => _currentForce;
    public float MaxForce;
    public float MaxSpeed;
    private Rigidbody _rigidbody;
    void Start()
    {
        _control = GetComponent<UserControl>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var currentSpeed = _rigidbody.linearVelocity.magnitude;
        if (currentSpeed > MaxSpeed)
        {
            return;
        }
        
        var rate =  currentSpeed / MaxSpeed;
        _currentForce = AnimationCurve.Evaluate(rate) * _control.Engine * MaxForce;
    }
}
