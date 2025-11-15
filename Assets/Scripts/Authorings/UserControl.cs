using Unity.Entities;
using UnityEngine;

public class UserControl : MonoBehaviour
{
    public class UserControlBaker : Baker<UserControl>
    {
        public override void Bake(UserControl authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new CartInputData { AllowControl = true });
        }
    }
}