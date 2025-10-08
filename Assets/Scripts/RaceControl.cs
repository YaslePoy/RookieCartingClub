using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class RaceControl : NetworkBehaviour
{
    public static RaceControl Singleton;
    public List<CartHandle> racers = new();
    public NetworkVariable<double> SessionTime = new();
    public NetworkVariable<double> PeriodEnd = new();
    public NetworkVariable<FixedString32Bytes> PeriodName = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Queue<IRacePeriod> racePeriods = new();
    public UIVM Uivm;
    public void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        // racers = racers.OrderByDescending(c => c.Laps.Count).ThenByDescending(i => i.Laps.Last().TotalLapTime).ToList();
        // var delta = SessionTime * 60 - (int)Time.time;
        // delta = delta > 0 ? delta : 0;

        if (IsServer)
        {
            if (Time.timeAsDouble > PeriodEnd.Value)
            {
                var period = racePeriods.Dequeue();
                racePeriods.Enqueue(period);

                period.Start(this);
            }
            
            SessionTime.Value = PeriodEnd.Value - Time.timeAsDouble ;
        }
        else if (IsClient)
        {
            var beforeEnd = TimeSpan.FromSeconds(SessionTime.Value);
            var viewTime = beforeEnd.ToString(@"mm\:ss");
            Uivm.SessionTime = viewTime;
            Uivm.SessionName = PeriodName.Value.Value;
        }
    }
}

public interface IRacePeriod
{
    void Start(RaceControl raceControl);
}

public class PrePeriod : IRacePeriod
{
    public double Duration;

    public void Start(RaceControl raceControl)
    {
        TrackPlacement.CurrentSpawn = 0;
        foreach (var racer in raceControl.racers)
        {
            racer.GetComponent<TrackPlacement>().PlaceOnTrack();
            racer.GetComponent<UserControl>().AllowControl = false;
            racer.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            racer.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        raceControl.PeriodName.Value = new FixedString32Bytes("Подготовка");
        raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
    }
}

public class RacePeriod : IRacePeriod
{
    public double Duration;

    public void Start(RaceControl raceControl)
    {
        foreach (var racer in raceControl.racers)
        {
            racer.GetComponent<UserControl>().AllowControl = true;
        }

        raceControl.PeriodName.Value = new FixedString32Bytes("Гонка");
        raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
    }
}