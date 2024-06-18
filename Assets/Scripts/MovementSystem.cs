using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Movement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (transform, movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Movement>>())
        {
            // Factor in drag
            movement.ValueRW.Value *= (1 - SystemAPI.Time.DeltaTime * movement.ValueRO.Drag);
            // Clamp vector to MaxSpeed
            if (math.length(movement.ValueRO.Value) > movement.ValueRO.MaxSpeed) movement.ValueRW.Value = math.normalize(movement.ValueRO.Value) * movement.ValueRO.MaxSpeed;

            // Update Transform
            transform.ValueRW = transform.ValueRO.Translate(new float3(movement.ValueRO.Value.x, movement.ValueRO.Value.y, 0f) * SystemAPI.Time.DeltaTime);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
