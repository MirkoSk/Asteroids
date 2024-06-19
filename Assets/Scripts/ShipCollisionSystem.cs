using System;
using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Projectile hit detection can create asteroid fragments that reside at (0,0,0) until its EntityCommandBuffer plays
// => We need to update the ship collision before that happens to prevent the ship from getting hit at (0,0,0) for no apparent reason
[UpdateBefore(typeof(ProjectileHitDetectionSystem))]
[BurstCompile]
partial class ShipCollisionSystem : SystemBase
{
    public event Action<int> OnDeath;

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (shipWorldTransform, ship, shipEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>, RefRW<Ship>>()
                .WithEntityAccess())
        {
            foreach (var (asteroidWorldTransform, asteroid) in
                SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Asteroid>>())
            {
                // Collision with an asteroid?
                if (math.distancesq(shipWorldTransform.ValueRO.Position, asteroidWorldTransform.ValueRO.Position) < 0.08f + asteroid.ValueRO.CollisionRadiusSQ)
                {
                    // Reduce lives
                    ship.ValueRW.Lives--;
                    ship.ValueRW.DeathTimestamp = SystemAPI.Time.ElapsedTime;

                    // Disable ship entity and all its children
                    foreach (var linkedEntity in SystemAPI.GetBuffer<LinkedEntityGroup>(shipEntity))
                    {
                        entityCommandBuffer.AddComponent<Disabled>(linkedEntity.Value);
                    }

                    // Notify GameObject land about the death
                    OnDeath?.Invoke(ship.ValueRO.Lives);

                    break;
                }
            }
        }

        entityCommandBuffer.Playback(EntityManager);

        entityCommandBuffer.Dispose();
    }
}
