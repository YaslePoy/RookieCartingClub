using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CartHandle : NetworkBehaviour
{
    public List<TrackLap> Laps = new();
    public static Action<CartHandle> NewCartConnected;

    [CanBeNull]
    public TrackLap FastestLaps => Laps.OrderBy(i => i.TotalLapTime).FirstOrDefault(i => i.IsValid && i.IsFinished);

    public TrackLap CurrentLap => Laps.Last();
    private int checkCount;

    public NetworkVariable<FixedString32Bytes> Nickname = new(new FixedString32Bytes(""),
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<int> PlayerId = new(0, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkCount = GameObject.Find("track_limits").GetComponentsInChildren<MeshCollider>().Length;
        print($"Segments count: {checkCount}");
        Laps.Add(new TrackLap(checkCount, Time.timeAsDouble));

        if (IsClient && IsOwner)
        {
            Nickname.Value = new FixedString32Bytes(SessionSetup.Nickname);
            PlayerId.Value = SessionSetup.Id;
        }

        RaceControl.Singleton.racers.Add(this);

        NewCartConnected?.Invoke(this);
    }

    public void PushCheckPoint(CheckPoint checkPoint)
    {
        var now = Time.timeAsDouble;

        var currentLap = CurrentLap;


        if (checkPoint.Index == 0)
        {
            Laps.Add(new TrackLap(checkCount, Time.timeAsDouble));
        }
        else
        {
            currentLap.SetupSegmentTime(now, checkPoint.Index);
        }
    }

    [Rpc(SendTo.Server)]
    public void TransferToPitRpc()
    {
        Debug.Log("TransferToPit");
        GetComponent<TrackPlacement>().Start();
    }

    public void OnDestroy()
    {
        RaceControl.Singleton.racers.Remove(this);
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
            int index = 0;
            foreach (var limitMesh in limits)
            {
                var go = limitMesh.gameObject;
                var mesh = limitMesh.mesh;
                var colider = go.AddComponent<MeshCollider>();
                colider.convex = true;
                colider.sharedMesh = mesh;
                colider.isTrigger = true;
                var check = go.AddComponent<CheckPoint>();
                check.Index = index++;
                go.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
#endif

public class TrackPositions : NetworkVariableBase
{
    private List<int> _positions = new();

    public List<int> Positions
    {
        get => _positions;
        set
        {
            if (!_positions.SequenceEqual(value))
            {
                SetDirty(true);
            }
            
            _positions = value;
        }
    }

    public override void WriteDelta(FastBufferWriter writer)
    {
        WriteField(writer);
    }

    public override void WriteField(FastBufferWriter writer)
    {
        writer.TryBeginWrite((Positions.Count + 1) * 4);
        writer.WriteValue(Positions.Count);
        foreach (var t in Positions)
        {
            writer.WriteValue(t);
        }
    }

    public override void ReadField(FastBufferReader reader)
    {
        reader.ReadValueSafe(out int count);
        Positions = new List<int>(count);
        Debug.Log($"Read {count} positions");
        for (int i = 0; i < count; i++)
        {
            reader.ReadValueSafe(out count);
            Positions.Add(count);
        }
    }

    public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    {
        ReadField(reader);
    }
}