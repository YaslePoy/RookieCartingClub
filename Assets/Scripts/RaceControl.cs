using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//todo
public class RaceControl : MonoBehaviour
{
    public static RaceControl Singleton;
    public List<CartHandle> racers = new();
    // public NetworkVariable<double> SessionTime = new();
    // public NetworkVariable<double> PeriodEnd = new();
    // public NetworkVariable<FixedString32Bytes> PeriodName = new();
    // public NetworkVariable<PeriodType> PeriodType = new();
    public UIVM Uivm;
    public IRacePeriod CurrentRacePeriod;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Queue<IRacePeriod> racePeriods = new();

    public TrackPositions TrackPositions = new();

    public void Start()
    {
        Singleton = this;
    }

    private void Update()
    {
        // if (IsClient)
        // {
        //     var beforeEnd = TimeSpan.FromSeconds(SessionTime.Value);
        //     var viewTime = beforeEnd.ToString(@"mm\:ss");
        //     Uivm.SessionTime = viewTime;
        //     Uivm.SessionName = PeriodName.Value.Value;
        //
        //     UpdatePositions();
        // }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // if (IsServer)
        // {
        //     if (Time.timeAsDouble > PeriodEnd.Value)
        //     {
        //         var period = racePeriods.Dequeue();
        //         racePeriods.Enqueue(period);
        //
        //         period.Start(this);
        //         CurrentRacePeriod = period;
        //     }
        //
        //     SessionTime.Value = PeriodEnd.Value - Time.timeAsDouble;
        //     CurrentRacePeriod.Update(this);
        // }
    }

    private void UpdatePositions()
    {
        var sb = new StringBuilder(256);
        for (var i = 0; i < TrackPositions.Positions.Count; i++)
        {
            var pos = i + 1;
            var nickname = racers.Find(c => c.PlayerId == TrackPositions.Positions[i]).Nickname.Value;
            sb.AppendLine($"{pos,2} | {nickname,-20}");
        }

        Uivm.Positions = sb.ToString();
    }

    public class RaceControlBaker : Baker<RaceControl>
    {
        public override void Bake(RaceControl authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var collection = AddBuffer<TrackPositionsCollection>(entity);

            var trackPositions = CreateAdditionalEntity(TransformUsageFlags.None, entityName: "TrackPositions");
            var pitPositions = CreateAdditionalEntity(TransformUsageFlags.None, entityName: "PitPositions");

            collection.Add(new TrackPositionsCollection { BufferEntity = trackPositions });
            collection.Add(new TrackPositionsCollection { BufferEntity = pitPositions });
            var trackBuffer = AddBuffer<TrackPlacementPosition>(trackPositions);
            var pitBuffer = AddBuffer<TrackPlacementPosition>(pitPositions);

            var positions = GameObject.Find("Starts").GetComponentsInChildren<Transform>()[1..];

            foreach (var transform in positions)
            {
                trackBuffer.Add(new TrackPlacementPosition
                {
                    Position = transform.position,
                    Rotation = transform.rotation                
                });
                
            }


            positions = (GameObject.Find("Pitline starts") ?? GameObject.Find("Starts"))
                .GetComponentsInChildren<Transform>()[1..];
            
            foreach (var transform in positions)
            {
                pitBuffer.Add(new TrackPlacementPosition
                {
                    Position = transform.position,
                    Rotation = transform.rotation
                });
            }
        }
    }
}

public struct TrackPlacementPosition : IBufferElementData
{
    public float3 Position;
    public quaternion Rotation;
}

public struct TrackPositionsCollection : IBufferElementData
{
    public Entity BufferEntity;
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
            racer.Laps.Clear();
        }

        // raceControl.PeriodName.Value = new FixedString32Bytes("Подготовка");
        // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
        // raceControl.PeriodType.Value = PeriodType.PreRace;
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
        foreach (var racer in raceControl.racers) racer.GetComponent<UserControl>().AllowControl = true;

        // raceControl.PeriodType.Value = PeriodType.Race;
        // raceControl.PeriodName.Value = new FixedString32Bytes("Гонка");
        // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;

        CartHandle.NewCartConnected = handle =>
        {
            handle.GetComponent<TrackPlacement>().PlaceInPits();
            handle.GetComponent<UserControl>().AllowControl = true;
            handle.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            handle.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        };
    }

    public void Update(RaceControl raceControl)
    {
        var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
            .ThenBy(i => (i.CurrentLap ?? TrackLap.Null).LastSegmentIndex).ToList();
        raceControl.racers = racersOrder;
        raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
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

        // raceControl.PeriodType.Value = PeriodType.Practice;
        // raceControl.PeriodName.Value = new FixedString32Bytes("Практика");
        // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;

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
        raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
    }
}

public class FinishPeriod : IRacePeriod
{
    public int CurrentLap;
    public double Duration;
    public bool IsLeaderFinished;

    public void Start(RaceControl raceControl)
    {
        // raceControl.PeriodType.Value = PeriodType.Finish;
        // raceControl.PeriodName.Value = new FixedString32Bytes("🏁 Финиш");
        // raceControl.PeriodEnd.Value = Duration + Time.timeAsDouble;
        if (raceControl.racers.FirstOrDefault() is { } racer) CurrentLap = racer.Laps.Count;
    }

    public void Update(RaceControl raceControl)
    {
        var racersOrder = raceControl.racers.OrderByDescending(i => i.Laps.Count)
            .ThenBy(i => i.CurrentLap.LastSegmentIndex).ToList();
        raceControl.racers = racersOrder;
        raceControl.TrackPositions.Positions = racersOrder.Select(i => i.PlayerId).ToList();
    }
}

public class TrackPositions
{
    private List<int> _positions = new();

    public List<int> Positions
    {
        get => _positions;
        set
        {
            // if (!_positions.SequenceEqual(value))
                // SetDirty(true);
            _positions = value;
        }
    }

    // public override void WriteDelta(FastBufferWriter writer)
    // {
    //     WriteField(writer);
    // }
    //
    // public override void WriteField(FastBufferWriter writer)
    // {
    //     Debug.Log("Writing track positions");
    //     writer.TryBeginWrite((Positions.Count + 1) * 4);
    //     writer.WriteValue(Positions.Count);
    //     foreach (var t in Positions) writer.WriteValue(t);
    // }
    //
    // public override void ReadField(FastBufferReader reader)
    // {
    //     reader.ReadValueSafe(out int count);
    //     Positions = new List<int>(count);
    //     Debug.Log($"Read {count} positions");
    //     for (var i = 0; i < count; i++)
    //     {
    //         reader.ReadValueSafe(out int p);
    //         Positions.Add(p);
    //     }
    // }
    //
    // public override void ReadDelta(FastBufferReader reader, bool keepDirtyDelta)
    // {
    //     ReadField(reader);
    // }
}