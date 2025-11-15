using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
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
}