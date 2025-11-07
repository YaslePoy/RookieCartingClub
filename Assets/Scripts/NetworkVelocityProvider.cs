using Unity.Netcode;
using UnityEngine;

public class NetworkVelocityProvider : NetworkBehaviour
{
    public NetworkVariable<Vector3> _velocity = new(Vector3.zero);

    private Vector3 _lastPosition;
    public Vector3 Velocity => _velocity.Value;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        var currentPosition = transform.position;
        var newVel = (currentPosition - _lastPosition) / Time.fixedDeltaTime;

        _velocity.Value = newVel;
        _lastPosition = currentPosition;
    }
}