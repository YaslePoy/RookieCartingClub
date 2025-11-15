using Unity.Entities;
using UnityEngine;

public class VelocityProvider : MonoBehaviour
{
    private Vector3 _lastPosition;
    public Vector3 Velocity { get; private set; }

    private void Start()
    {
        Velocity = Vector3.zero;
        _lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        var currentPosition = transform.position;
        var newVel = (currentPosition - _lastPosition) / Time.fixedDeltaTime;

        Velocity = newVel;
        _lastPosition = currentPosition;
    }

    public class VelocityProviderBaker : Baker<VelocityProvider>
    {
        public override void Bake(VelocityProvider authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<LocalVelocity>(entity);
        }
    }
}