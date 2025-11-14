using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEditor;
using UnityEngine;
using MeshCollider = UnityEngine.MeshCollider;

public class CartHandle : MonoBehaviour
{
    public static Action<CartHandle> NewCartConnected;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // checkCount = GameObject.Find("track_limits").GetComponentsInChildren<MeshCollider>().Length;
        print($"Segments count: {CheckCount}");
        Laps.Add(new TrackLap(CheckCount, Time.timeAsDouble));

        // if (IsClient && IsOwner)
        // {
        //     Nickname.Value = new FixedString32Bytes(SessionSetup.Nickname);
        //     PlayerId = SessionSetup.Id;
        // }

        RaceControl.Singleton.racers.Add(this);

        NewCartConnected?.Invoke(this);
    }

    public void OnDestroy()
    {
        RaceControl.Singleton.racers.Remove(this);
    }

    public void PushCheckPoint(CheckPointData checkPoint)
    {
        var now = Time.timeAsDouble;

        var currentLap = CurrentLap;


        if (checkPoint.Index == 0)
            Laps.Add(new TrackLap(CheckCount, Time.timeAsDouble));
        else
            currentLap.SetupSegmentTime(now, checkPoint.Index);
    }
    
}


#if UNITY_EDITOR
[CustomEditor(typeof(CartHandle))]
public class CartHandleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Convert"))
        {
            var limits = GameObject.Find("track_limits").GetComponentsInChildren<MeshFilter>();
            var index = 0;
            foreach (var limitMesh in limits)
            {
                var go = limitMesh.gameObject;
                go.GetComponent<MeshRenderer>().enabled = false;
                var col = go.AddComponent<MeshCollider>();
                col.convex = true;
                col.isTrigger = true;
                col.sharedMesh = limitMesh.sharedMesh;
                var data = go.AddComponent<CheckPoint>();
                data.Index = index++;
            }
        }

        if (GUILayout.Button("border"))
        {
            var limits = GameObject.Find("ground_colliders").GetComponentsInChildren<MeshCollider>();

            foreach (var limitMesh in limits) limitMesh.isTrigger = false;
            // var go = limitMesh.gameObject;
            // var mesh = limitMesh.mesh;
            // var colider = go.AddComponent<MeshCollider>();
            // colider.convex = true;
            // colider.sharedMesh = mesh;
            // colider.isTrigger = true;
            // go.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
#endif


public class CartHandleBaker : Baker<CartHandle>
{
    public override void Bake(CartHandle authoring)
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
    }
}

public struct CartData : IComponentData
{
    public FixedString32Bytes Nickname;
    public int PlayerId;
}

public struct ForceApplier : IComponentData
{
}

public struct FinalForceRequest : IBufferElementData
{
    public float3 Force;
    public float3 Position;
}

public struct CurrentContactingSegment : IBufferElementData
{
    public int Index;
}

public struct NewContactingSegment : IBufferElementData
{
    public int Index;
}