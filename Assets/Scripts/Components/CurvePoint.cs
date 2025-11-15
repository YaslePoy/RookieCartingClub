using Unity.Entities;
using Unity.Mathematics;

namespace RookieCartingClub.Components
{
    public struct CurvePoint : IBufferElementData
    {
        public float2 Value;
    }
}