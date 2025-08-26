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
        _velocity =  (currentPosition - _lastPosition) / Time.deltaTime;
        _lastPosition = currentPosition;
    }
}
