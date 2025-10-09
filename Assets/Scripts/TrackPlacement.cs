using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

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
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
        GetComponent<Rigidbody>().freezeRotation = true;

        StartCoroutine(ResetRotation());
    }

    IEnumerator ResetRotation()
    {
        yield return null;
        GetComponent<Rigidbody>().freezeRotation = false;
    }
    
    public void PlaceOnTrack()
    {
        if (!IsServer) return;

        var collection = GameObject.Find("Starts").GetComponentsInChildren<Transform>()[1..];
        var transform = collection[CurrentSpawn++ % collection.Length];
        print($"Spawning on {CurrentSpawn}");
        this.transform.SetPositionAndRotation(transform.position, transform.rotation);
        GetComponent<Rigidbody>().freezeRotation = true;

        StartCoroutine(ResetRotation());
    }
}