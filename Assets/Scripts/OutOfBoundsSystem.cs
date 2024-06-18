using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct OutOfBoundsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Movement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        foreach (var (transform, movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Movement>>())
        {
            if (math.abs(transform.ValueRO.Position.x) > gameConfig.PlayAreaBounds.x)
            {
                transform.ValueRW.Position = new float3(-transform.ValueRO.Position.x, transform.ValueRO.Position.y, transform.ValueRO.Position.z);
            }
            else if (math.abs(transform.ValueRO.Position.y) > gameConfig.PlayAreaBounds.y)
            {
                transform.ValueRW.Position = new float3(transform.ValueRO.Position.x, -transform.ValueRO.Position.y, transform.ValueRO.Position.z);
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
