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
            var transform = GameObject.Find($"Start_{CurrentSpawn++}");
            this.transform.position =  transform.transform.position;
            this.transform.rotation = transform.transform.rotation;
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}