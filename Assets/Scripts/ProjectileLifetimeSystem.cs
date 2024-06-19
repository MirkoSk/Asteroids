using System.Diagnostics;
using Asteroids;
using Unity.Burst;
using Unity.Entities;

partial struct ProjectileLifetimeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Projectile>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (projectile, entity) in SystemAPI.Query<RefRO<Projectile>>().WithEntityAccess())
        {
            if (SystemAPI.Time.ElapsedTime > projectile.ValueRO.SpawnTime + projectile.ValueRO.Lifetime)
            {
                SystemAPI.GetComponentRW<Turret>(projectile.ValueRO.Turret).ValueRW.ProjectilesCurrentlyOnScreen--;
                entityCommandBuffer.DestroyEntity(entity);
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
