using UnityEngine;

public class VelocityProvider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 Velocity => _velocity;
    private Vector3 _velocity;
    private Vector3 _lastPosition;
    void Start()
    {
        _velocity = Vector3.zero;
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var currentPosition = transform.position;
        _velocity =  (currentPosition - _lastPosition) / Time.deltaTime;
        _lastPosition = currentPosition;
    }
}
