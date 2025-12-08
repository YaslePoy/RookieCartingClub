using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RookieCartingClub.Components;
using RookieCartingClub.Systems;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace RookieCartingClub.Authoring
{
    public class CartHandleAuthoring : MonoBehaviour
    {
        public static Action<CartHandleAuthoring> NewCartConnected;

        // public NetworkVariable<FixedString32Bytes> Nickname = new(new FixedString32Bytes(""),
        //     NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        //
        // public NetworkVariable<int> PlayerId = new(0, NetworkVariableReadPermission.Everyone,
        //     NetworkVariableWritePermission.Owner);


        public int CheckCount;
        public List<TrackLap> Laps = new();

        [CanBeNull]
        public TrackLap FastestLaps => Laps.OrderBy(i => i.TotalLapTime).FirstOrDefault(i => i.IsValid && i.IsFinished);

        public TrackLap CurrentLap => Laps.LastOrDefault();
        public FixedString32Bytes Nickname;
        public int PlayerId;

        // // Start is called once before the first execution of Update after the MonoBehaviour is created
        // public void Start()
        // {
        //     print($"Segments count: {CheckCount}");
        //     Laps.Add(new TrackLap(CheckCount, Time.timeAsDouble));
        //
        //
        //     RaceControlAuthoring.Singleton.racers.Add(this);
        //
        //     NewCartConnected?.Invoke(this);
        // }

        public void PushCheckPoint(CheckPointData checkPoint)
        {
            var now = Time.timeAsDouble;

            var currentLap = CurrentLap;


            if (checkPoint.Index == 0)
                Laps.Add(new TrackLap(CheckCount, Time.timeAsDouble));
            else
                currentLap.SetupSegmentTime(now, checkPoint.Index);
        }
    
        public class CartHandleBaker : Baker<CartHandleAuthoring>
        {
            public override void Bake(CartHandleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new CartData
                {
                    Nickname = authoring.Nickname,
                    PlayerId = authoring.PlayerId
                });
                AddComponent<ForceApplier>(entity);
                AddBuffer<FinalForceRequest>(entity);
                AddBuffer<CurrentContactingSegment>(entity);
                AddBuffer<NewContactingSegment>(entity);
                AddComponent<EnableSimulate>(entity);
                SetComponentEnabled<EnableSimulate>(entity, false);
                AddComponent<TrackPlacementRequest>(entity);
                SetComponentEnabled<TrackPlacementRequest>(entity, false);
                AddComponent<WasTeleported>(entity);
                SetComponentEnabled<WasTeleported>(entity, true);
            }
        }
    }
}


// #if UNITY_EDITOR
// [CustomEditor(typeof(CartHandle))]
// public class CartHandleEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//         if (GUILayout.Button("Convert"))
//         {
//             var limits = GameObject.Find("track_limits").GetComponentsInChildren<MeshFilter>();
//             var index = 0;
//             foreach (var limitMesh in limits)
//             {
//                 var go = limitMesh.gameObject;
//                 go.GetComponent<MeshRenderer>().enabled = false;
//                 var col = go.AddComponent<MeshCollider>();
//                 col.convex = true;
//                 col.isTrigger = true;
//                 col.sharedMesh = limitMesh.sharedMesh;
//                 var data = go.AddComponent<CheckPoint>();
//                 data.Index = index++;
//             }
//         }
//
//         if (GUILayout.Button("border"))
//         {
//             var limits = GameObject.Find("ground_colliders").GetComponentsInChildren<MeshCollider>();
//
//             foreach (var limitMesh in limits) limitMesh.isTrigger = false;
//             // var go = limitMesh.gameObject;
//             // var mesh = limitMesh.mesh;
//             // var colider = go.AddComponent<MeshCollider>();
//             // colider.convex = true;
//             // colider.sharedMesh = mesh;
//             // colider.isTrigger = true;
//             // go.GetComponent<MeshRenderer>().enabled = false;
//         }
//     }
//
//
// }
// #endif