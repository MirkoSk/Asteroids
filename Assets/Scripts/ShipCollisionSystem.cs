using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Projectile hit detection can create asteroid fragments that reside at (0,0,0) until its EntityCommandBuffer plays
// => We need to update the ship collision before that happens to prevent the ship from getting hit at (0,0,0) for no apparent reason
[UpdateBefore(typeof(ProjectileHitDetectionSystem))]
partial struct ShipCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ship>();
        state.RequireForUpdate<Asteroid>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (shipWorldTransform, shipEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>>()
                .WithAll<Ship>()
                .WithEntityAccess())
        {
            foreach (var (asteroidWorldTransform, asteroid) in
                SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Asteroid>>())
            {
                // Collision with an asteroid?
                if (math.distancesq(shipWorldTransform.ValueRO.Position, asteroidWorldTransform.ValueRO.Position) < 0.08f + asteroid.ValueRO.CollisionRadiusSQ)
                {
                    entityCommandBuffer.DestroyEntity(shipEntity);
                    // TODO: Decrease lives left and respawn ship after a delay
                    break;
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
