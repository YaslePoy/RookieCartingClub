using System;
using Unity.Netcode;
using UnityEngine;

public class TrackPlacement : NetworkBehaviour
{
    public static int CurrentSpawn;

    public void Start()
    {
        if (this.IsServer)
        {
            var collection = GameObject.Find("Starts").GetComponentsInChildren<Transform>()[1..];
            var transform = collection[CurrentSpawn++ % collection.Length];
            print($"Spawning on {CurrentSpawn}");
            this.transform.position =  transform.position;
            this.transform.rotation = transform.rotation;
        }
    }
}