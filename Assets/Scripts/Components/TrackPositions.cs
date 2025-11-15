using System.Collections.Generic;

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