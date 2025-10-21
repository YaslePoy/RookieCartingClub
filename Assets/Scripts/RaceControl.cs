using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public NetworkVariable<PeriodType> PeriodType = new();
    public TrackPositions TrackPositions = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Queue<IRacePeriod> racePeriods = new();
    public IRacePeriod CurrentRacePeriod;
    public UIVM Uivm;

    public void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsServer)
        {
            if (Time.timeAsDouble > PeriodEnd.Value)
            {
                var period = racePeriods.Dequeue();
                racePeriods.Enqueue(period);

                period.Start(this);
                CurrentRacePeriod = period;
            }

            SessionTime.Value = PeriodEnd.Value - Time.timeAsDouble;
            CurrentRacePeriod.Update(this);
        }
        
    }

    private void Update()
    {
        if (IsClient)
        {
            var beforeEnd = TimeSpan.FromSeconds(SessionTime.Value);
            var viewTime = beforeEnd.ToString(@"mm\:ss");
            Uivm.SessionTime = viewTime;
            Uivm.SessionName = PeriodName.Value.Value;
            
            UpdatePositions();
            
        }
    }

    private void UpdatePositions()
    {
        var sb = new StringBuilder(256);
        for (int i = 0; i < TrackPositions.Positions.Count; i++)
        {
            var pos = i + 1;
            var nickname = racers.Find(c => c.PlayerId.Value == TrackPositions.Positions[i]).Nickname.Value;
            sb.AppendLine($"{pos, 2} | {nickname, -20}");
        }
        
        Uivm.Positions = sb.ToString();
    }
}

public enum PeriodType
{
    Practice,
    Race,
    PreRace,
    Qualification,
    Finish
}

public interface IRacePeriod
{
    void Start(RaceControl raceControl);
    void Update(RaceControl raceControl);
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
        raceControl.PeriodType.Value = PeriodType.PreRace;
        CartHandle.NewCartConnected = handle =>
        {
            handle.GetComponent<TrackPlacement>().PlaceOnTrack();
            handle.GetComponent<UserControl>().AllowControl = false;
            handle.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            handle.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        };
    }

    public void Update(RaceControl raceControl)
    {
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
        
        raceControl.PeriodType.Value = PeriodType.Race;
        raceControl.PeriodName.Value = new FixedString32Bytes("Гонка");
        raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;

        CartHandle.NewCartConnected = handle =>
        {
            handle.GetComponent<TrackPlacement>().PlaceOnTrack();
            handle.GetComponent<UserControl>().AllowControl = false;
            handle.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            handle.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        };
    }

    public void Update(RaceControl raceControl)
    {
        var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
            .ThenBy(i => i.CurrentLap.LastSegmentIndex).ToList();
        raceControl.racers = racersOrder;
        raceControl.TrackPositions.Positions =  racersOrder.Select(i => i.PlayerId.Value).ToList();
    }
}

public class PracticePeriod : IRacePeriod
{
    public double Duration;

    public void Start(RaceControl raceControl)
    {
        TrackPlacement.CurrentSpawn = 0;
        foreach (var racer in raceControl.racers)
        {
            racer.GetComponent<TrackPlacement>().PlaceInPits();
            racer.GetComponent<UserControl>().AllowControl = true;
            var rigidbody = racer.GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        raceControl.PeriodType.Value = PeriodType.Practice;
        raceControl.PeriodName.Value = new FixedString32Bytes("Практика");
        raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;

        CartHandle.NewCartConnected = handle =>
        {
            handle.GetComponent<TrackPlacement>().PlaceInPits();
            handle.GetComponent<UserControl>().AllowControl = true;
            var rigidbody = handle.GetComponent<Rigidbody>();
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        };
    }

    public void Update(RaceControl raceControl)
    {
        var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
            .ThenBy(i => i.CurrentLap.LastSegmentIndex).ToList();
        raceControl.racers = racersOrder;
        raceControl.TrackPositions.Positions =  racersOrder.Select(i => i.PlayerId.Value).ToList();
    }
}

public class FinishPeriod : IRacePeriod
{
    public double Duration;

    public void Start(RaceControl raceControl)
    {
        raceControl.PeriodType.Value = PeriodType.Finish;
        raceControl.PeriodName.Value = new FixedString32Bytes("Финиш");
        raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
    }

    public void Update(RaceControl raceControl)
    {
    }
}