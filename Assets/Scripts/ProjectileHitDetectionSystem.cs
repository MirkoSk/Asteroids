using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ProjectileHitDetectionSystem : ISystem
{
    uint updateCounter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Projectile>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var random = Random.CreateFromIndex(updateCounter++);

        foreach (var (projectileTransform, projectileEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<Projectile>()
            .WithEntityAccess())
        {
            foreach (var (asteroidTransform, asteroidMovement, asteroid, asteroidEntity) in 
                SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Movement>, RefRO<Asteroid>>()
                .WithEntityAccess())
            {
                // A projectile hit an asteroid
                if (math.distancesq(projectileTransform.ValueRO.Position, asteroidTransform.ValueRO.Position) < asteroid.ValueRO.CollisionRadiusSQ)
                {
                    // Destroy projectile
                    entityCommandBuffer.DestroyEntity(projectileEntity);

                    // Spawn smaller asteroid fragments if linked
                    if (asteroid.ValueRO.FragmentsAmount > 0 && asteroid.ValueRO.FragmentPrefab != null)
                    {
                        for (int i = 0; i < asteroid.ValueRO.FragmentsAmount; i++)
                        {
                            Entity newAsteroidEntity = entityCommandBuffer.Instantiate(asteroid.ValueRO.FragmentPrefab);
                            entityCommandBuffer.SetComponent(newAsteroidEntity, new LocalTransform
                            {
                                Position = asteroidTransform.ValueRO.Position,
                                Rotation = quaternion.identity,
                                Scale = 1f
                            });
                            // Remark: Getting the values of the prefab since EntityCommandBuffer.SetComponent overrides all fields
                            var spawnedPrefabMovement = SystemAPI.GetComponentRO<Movement>(asteroid.ValueRO.FragmentPrefab);
                            entityCommandBuffer.SetComponent(newAsteroidEntity, new Movement
                            {
                                Value = math.normalize(random.NextFloat2(-1f, 1f)) * random.NextFloat(math.length(asteroidMovement.ValueRO.Value) * 1.5f, math.length(asteroidMovement.ValueRO.Value) * 2f),
                                Drag = spawnedPrefabMovement.ValueRO.Drag,
                                MaxSpeed = spawnedPrefabMovement.ValueRO.MaxSpeed
                            });
                        }
                    }

                    // Destroy asteroid
                    entityCommandBuffer.DestroyEntity(asteroidEntity);
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
