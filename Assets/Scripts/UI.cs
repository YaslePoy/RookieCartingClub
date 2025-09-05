using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public VelocityProvider VelocityProvider;
    public UIVM Uivm;
    public CartHandle Cart;
    
     void Update()
     {
         var velocity = VelocityProvider.Velocity.magnitude;
         Uivm.Speed = velocity;
         Uivm.UpdateSpeedKmh();
         Uivm.LapTime = $"{TimeSpan.FromSeconds(Time.timeAsDouble - Cart.LapStart):g}";
         if (Cart.FastestLaps.Count != 0)
         {
             var sector = Cart.Laps.Last().Last();
             var delta = sector - Cart.FastestLaps[Cart.Laps.Last().Count - 1];
             var formated = Math.Round(delta, 2).ToString();
             Uivm.Delta = formated;
         }
     }
}