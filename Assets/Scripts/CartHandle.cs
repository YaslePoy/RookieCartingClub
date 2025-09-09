using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CartHandle : MonoBehaviour
{
    public List<List<double>> Laps = new();
    public List<double> FastestLaps;
    public double LapStart = -1;
    private List<List<double>> _invalidLaps = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PushCheckPoint(CheckPoint checkPoint)
    {
            var now = Time.timeAsDouble;
        
        if (checkPoint.Index == 0)
        {

            if (Laps.Count > 0)
            {
                Laps.Last().Add(now - LapStart);
                print($"Lap time {TimeSpan.FromSeconds(Laps.Last().Last()):g}");
            }

            LapStart = now;
            var times = new List<double>();
            Laps.Add(times);

            if (Laps.Count > 1)
            {
                FastestLaps = Laps.OrderBy(i => i.Last()).First(list => list.Count != 0 && !_invalidLaps.Contains(list));
            }
            
            
        }
        else
        {
            if (Laps.Count > 0 && Laps.Last()?.Count != checkPoint.Index - 1)
            {
                print("Invalid lap");
                while (Laps.Last()?.Count < checkPoint.Index - 1)
                {
                    Laps.Last().Add(0);
                }
                _invalidLaps.Add(Laps.Last());
            }

            if (Laps.Count > 0)
            {
                Laps.Last().Add(now - LapStart);
            }
        }
    }
}

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