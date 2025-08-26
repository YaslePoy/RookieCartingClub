using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public VelocityProvider VelocityProvider;
    public UIVM Uivm;

     void Update()
     {
         var velocity = VelocityProvider.Velocity.magnitude;
         Uivm.Speed = velocity;
         Uivm.UpdateSpeedKmh();
     }
}