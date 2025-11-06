using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(ForceApplySystem))]
public partial struct ForceSummarySystem : ISystem
{
    private BufferLookup<FinalForceRequest> _finalRequestLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CartWheel>();
        _finalRequestLookup = state.GetBufferLookup<FinalForceRequest>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _finalRequestLookup.Update(ref state);

        new ForceSummaryJob
        {
            FinalRequestLookup = _finalRequestLookup
        }.Schedule();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

[BurstCompile]
public partial struct ForceSummaryJob : IJobEntity
{
    public BufferLookup<FinalForceRequest> FinalRequestLookup;

    private void Execute(ref DynamicBuffer<ForceApplyRequest> requests, CartWheel wheelData, Parent parent,
        LocalToWorld position)
    {
        var sumForce = float3.zero;
        for (var i = 0; i < requests.Length; i++)
        {
            sumForce += requests[i].Force;
        }

        sumForce *= wheelData.ForcePart * wheelData.Friction * wheelData.Mass;

        var length = sumForce.Length();
        if (length == 0)
            return;
        
        if (length > wheelData.MaxResistance)
        {
            sumForce *= wheelData.MaxResistance / length;
        }

        var finalBuffer = FinalRequestLookup[parent.Value];
        finalBuffer.Add(new FinalForceRequest
        {
            Force = sumForce,
            Position = position.Position
        });
        
        requests.Clear();
    }
}