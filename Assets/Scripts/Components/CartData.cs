using Unity.Collections;
using Unity.Entities;

public struct CartData : IComponentData
{
    public FixedString32Bytes Nickname;
    public int PlayerId;
}
