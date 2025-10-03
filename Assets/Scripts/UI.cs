using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NetworkVelocityProvider VelocityProvider;
    public UIVM Uivm;
    public CartHandle Cart;

    private void Start()
    {
        VelocityProvider = Cart.gameObject.GetComponent<NetworkVelocityProvider>();
    }

    void Update()
    {
        var velocity = VelocityProvider.Velocity.magnitude;
        Uivm.Speed = velocity;
        Uivm.UpdateSpeedKmh();
        Uivm.LapTime = $"{TimeSpan.FromSeconds(Time.timeAsDouble - Cart.LapStart):g}";
        if (Cart.FastestLaps is not null)
        {
            try
            {
                var delta = Cart.LastLap.Delta(Cart.FastestLaps);
                var formated = $"{Math.Round(delta, 2):N2}";
                Uivm.Delta = formated;
            }
            catch
            {
            }
        }

        Uivm.Lap = Cart.Laps.Count;
    }
}