using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

//todo
public class TrackPlacement : NetworkBehaviour
{
    public static int CurrentSpawn;

    public void Start()
    {
        PlaceInPits();
    }

    public void PlaceInPits()
    {
        if (!IsServer) return;

        var collection = (GameObject.Find("Pitline starts") ?? GameObject.Find("Starts"))
            .GetComponentsInChildren<Transform>()[1..];
        var transform = collection[CurrentSpawn++ % collection.Length];
        print($"Spawning on {CurrentSpawn}");
        var rb = GetComponent<Rigidbody>();
        rb.Move(transform.position, transform.rotation);
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }


    public void PlaceOnTrack()
    {
        if (!IsServer) return;

        var collection = GameObject.Find("Starts").GetComponentsInChildren<Transform>()[1..];
        var transform = collection[CurrentSpawn++ % collection.Length];
        print($"Spawning on {CurrentSpawn}");
        var rb = GetComponent<Rigidbody>();
        rb.Move(transform.position, transform.rotation);
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }
}