using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CartHandle : NetworkBehaviour
{
    public List<TrackLap> Laps = new();

    [CanBeNull]
    public TrackLap FastestLaps => Laps.OrderBy(i => i.TotalLapTime).FirstOrDefault(i => i.IsValid && i.IsFinished);
    public TrackLap CurrentLap => Laps.Last();
    private int checkCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkCount = GameObject.Find("track_limits").GetComponentsInChildren<MeshCollider>().Length;
        print($"Segments count: {checkCount}");
        Laps.Add(new TrackLap(checkCount, Time.timeAsDouble));
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