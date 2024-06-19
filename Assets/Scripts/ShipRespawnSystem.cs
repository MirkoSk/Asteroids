using System.Diagnostics;
using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ShipRespawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        foreach (var (shipTransform, ship, shipMovement, shipEntity) in 
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<Ship>, RefRW<Movement>>()
                .WithAll<Disabled>()
                .WithEntityAccess())
        {
            if (ship.ValueRO.Lives > 0 && SystemAPI.Time.ElapsedTime - ship.ValueRO.DeathTimestamp >= gameConfig.ShipRespawnDuration)
            {
                // Move ship back to center of the screen
                shipTransform.ValueRW.Position = float3.zero;
                shipTransform.ValueRW.Rotation = quaternion.identity;

                // Reset velocity
                shipMovement.ValueRW.Value = float2.zero;

                // Enable ship entity and all its children
                foreach (var linkedEntity in SystemAPI.GetBuffer<LinkedEntityGroup>(shipEntity))
                {
                    entityCommandBuffer.RemoveComponent<Disabled>(linkedEntity.Value);
                }
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);

        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
