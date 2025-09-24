using Unity.Netcode;
using UnityEngine;

public class NetworkVelocityProvider : NetworkBehaviour
{
    public Vector3 Velocity => _velocity.Value;

    public NetworkVariable<Vector3> _velocity = new(Vector3.zero, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private Vector3 _lastPosition;

    void Start()
    {
        _lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!IsServer)
            return;
        
        var currentPosition = transform.position;
        var newVel = (currentPosition - _lastPosition) / Time.fixedDeltaTime;
        if (name.Contains("cart"))
        {
            print($"speed: {newVel.magnitude}");
        }

        _velocity.Value = newVel;
        _lastPosition = currentPosition;
    }
}