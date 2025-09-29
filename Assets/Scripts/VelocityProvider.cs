using UnityEngine;

public class VelocityProvider : MonoBehaviour
{
    public Vector3 Velocity => _velocity;
    private Vector3 _velocity;
    private Vector3 _lastPosition;

    void Start()
    {
        _velocity = Vector3.zero;
        _lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        var currentPosition = transform.position;
        var newVel = (currentPosition - _lastPosition) / Time.fixedDeltaTime;

        _velocity = newVel;
        _lastPosition = currentPosition;
    }
}