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
    public event System.Action<int> OnDeath;

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        var random = Random.CreateFromIndex(123);

        foreach (var (shipWorldTransform, ship, shipEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>, RefRW<Ship>>()
                .WithEntityAccess())
        {
            foreach (var (asteroidWorldTransform, asteroid, asteroidMovement, asteroidEntity) in
                SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Asteroid>, RefRO<Movement>>()
                .WithEntityAccess())
            {
                // Collision with an asteroid?
                if (math.distancesq(shipWorldTransform.ValueRO.Position, asteroidWorldTransform.ValueRO.Position) < 0.06f + asteroid.ValueRO.CollisionRadiusSQ)
                {
                    // Reduce lives
                    ship.ValueRW.Lives--;
                    ship.ValueRW.DeathTimestamp = SystemAPI.Time.ElapsedTime;

                    // Disable ship entity and all its children
                    foreach (var linkedEntity in SystemAPI.GetBuffer<LinkedEntityGroup>(shipEntity))
                    {
                        entityCommandBuffer.AddComponent<Disabled>(linkedEntity.Value);
                    }

                    // Spawn smaller asteroid fragments if linked
                    if (asteroid.ValueRO.FragmentsAmount > 0 && asteroid.ValueRO.FragmentPrefab != null)
                    {
                        for (int i = 0; i < asteroid.ValueRO.FragmentsAmount; i++)
                        {
                            Entity newAsteroidEntity = entityCommandBuffer.Instantiate(asteroid.ValueRO.FragmentPrefab);
                            entityCommandBuffer.SetComponent(newAsteroidEntity, new LocalTransform
                            {
                                Position = asteroidWorldTransform.ValueRO.Position,
                                Rotation = quaternion.identity,
                                Scale = 1f
                            });
                            // Remark: Getting the values of the prefab since EntityCommandBuffer.SetComponent overrides all fields
                            var spawnedPrefabMovement = SystemAPI.GetComponentRO<Movement>(asteroid.ValueRO.FragmentPrefab);
                            entityCommandBuffer.SetComponent(newAsteroidEntity, new Movement
                            {
                                Value = math.normalize(random.NextFloat2(-1f, 1f)) * random.NextFloat(math.length(asteroidMovement.ValueRO.Value) * 1.1f, math.length(asteroidMovement.ValueRO.Value) * 1.5f),
                                Drag = spawnedPrefabMovement.ValueRO.Drag,
                                MaxSpeed = spawnedPrefabMovement.ValueRO.MaxSpeed
                            });
                        }
                    }

                    // Add player score
                    SystemAPI.GetSingletonRW<PlayerScore>().ValueRW.CurrentValue += SystemAPI.GetComponentRO<Score>(asteroidEntity).ValueRO.Value;

                    // Destroy asteroid
                    entityCommandBuffer.DestroyEntity(asteroidEntity);

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
