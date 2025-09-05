using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CartHandle : MonoBehaviour
{
    public List<TimeSpan> Laps = new List<TimeSpan>();

    private double _lapStart = -1;

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
        if (checkPoint.Index == 0)
        {
            var now = Time.timeAsDouble;

            if (_lapStart > 0)
            {
                Laps.Add(TimeSpan.FromSeconds(now - _lapStart));
                print($"Lap time {Laps.Last():g}");
            }

            _lapStart = now;
            print($"Lap start: {TimeSpan.FromSeconds(_lapStart):g}");
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