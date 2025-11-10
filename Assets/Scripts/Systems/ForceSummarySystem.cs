using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

    public void OnUpdate(ref SystemState state)
    {
        _finalRequestLookup.Update(ref state);
        var list = new NativeList<DebugLine>(Allocator.TempJob);
        new ForceSummaryJob
        {
            FinalRequestLookup = _finalRequestLookup,
            DebugLines = list
        }.Schedule(state.Dependency).Complete();


        for (var i = 0; i < list.Length; i++)
        {
            Debug.DrawLine(list[i].P1, list[i].P2, Color.white);
            
        }

        list.Dispose();
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
    public NativeList<DebugLine> DebugLines;

    private void Execute(ref DynamicBuffer<ForceApplyRequest> requests, ref CartWheel wheelData, Parent parent,
        LocalToWorld position)
    {
        var sumForce = float3.zero;
        for (var i = 0; i < requests.Length; i++) sumForce += requests[i].Force;

        sumForce *= wheelData.ForcePart * wheelData.Friction * wheelData.Mass;

        var length = math.length(sumForce);
        if (length == 0)
            return;

        if (length > wheelData.MaxResistance) sumForce *= wheelData.MaxResistance / length;

        var finalBuffer = FinalRequestLookup[parent.Value];
        finalBuffer.Add(new FinalForceRequest
        {
            Force = sumForce,
            Position = position.Position
        });
        
        wheelData.CurrentForce = sumForce;
        wheelData.ForceLen = math.length(sumForce);

        var startPos = position.Position + new float3(0, 1, 0);
        var normalized = sumForce / wheelData.MaxResistance;
        DebugLines.Add(new DebugLine
        {
            P1 = startPos, P2 = startPos + normalized
        });
        DebugLines.Add(new DebugLine
        {
            P1 = startPos, P2 = position.Position
        });
        requests.Clear();
    }
}

public struct DebugLine
{
    public float3 P1;
    public float3 P2;
}