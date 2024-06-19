using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(ShipCollisionSystem))]
partial class ProjectileHitDetectionSystem : SystemBase
{
    public event System.Action OnHit;

    uint updateCounter;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<Projectile>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var random = Random.CreateFromIndex(updateCounter++);

        foreach (var (projectileWorldTransform, projectile, projectileEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Projectile>>()
                .WithEntityAccess())
        {
            foreach (var (asteroidWorldTransform, asteroidMovement, asteroid, asteroidEntity) in 
                SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Movement>, RefRO<Asteroid>>()
                    .WithEntityAccess())
            {
                // Did this projectile hit an asteroid?
                if (math.distancesq(projectileWorldTransform.ValueRO.Position, asteroidWorldTransform.ValueRO.Position) < asteroid.ValueRO.CollisionRadiusSQ)
                {
                    // Destroy projectile
                    SystemAPI.GetComponentRW<Turret>(projectile.ValueRO.Turret).ValueRW.ProjectilesCurrentlyOnScreen--;
                    entityCommandBuffer.DestroyEntity(projectileEntity);

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

                    // Inform GameObject land
                    OnHit?.Invoke();

                    // Destroy asteroid
                    entityCommandBuffer.DestroyEntity(asteroidEntity);
                }
            }
        }

        entityCommandBuffer.Playback(EntityManager);

        entityCommandBuffer.Dispose();
    }
}
