using RookieCartingClub.Components;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class CartSpawnerAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        private class CartSpawnerAuthoringBaker : Baker<CartSpawnerAuthoring>
        {
            public override void Bake(CartSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new CartSpawner
                {
                    CartPrefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic)
                });
                AddComponent(entity, new ConnectRequest
                {
                    PlayerData = new CartData
                    {
                        PlayerId = 123
                    }
                });
            }
        }
    }
}