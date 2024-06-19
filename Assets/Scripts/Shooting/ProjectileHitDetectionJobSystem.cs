using Asteroids;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/// <remark>
/// This is my best attempt at transferring the projectile's hit detection into a job.
/// I wasn't able to eliminate two race conditions yet so I don't use this code.
/// </remark>
[BurstCompile]
[UpdateAfter(typeof(ShipCollisionSystem))]
partial struct ProjectileHitDetectionJobSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Projectile>();
        state.Enabled = false;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();


        foreach (var (projectileWorldTransform, projectile, projectileEntity) in 
            SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Projectile>>()
                .WithEntityAccess())
        {
            var job = new HitDetectionJob
            {
                entityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
                projectileWorldTransform = projectileWorldTransform.ValueRO,
                projectile = projectile.ValueRO,
                projectileTurret = SystemAPI.GetComponentRW<Turret>(projectile.ValueRO.Turret),
                projectileEntity = projectileEntity,
                playerScore = SystemAPI.GetSingletonRW<PlayerScore>()
            };
            job.Schedule();
        }
    }

    [BurstCompile]
    partial struct HitDetectionJob : IJobEntity
    {
        public EntityCommandBuffer entityCommandBuffer;
        public LocalToWorld projectileWorldTransform;
        public Projectile projectile;
        // TODO: How to make this thread-safe?
        [NativeDisableUnsafePtrRestriction] public RefRW<Turret> projectileTurret;
        public Entity projectileEntity;
        // TODO: How to make this thread-safe?
        [NativeDisableUnsafePtrRestriction] public RefRW<PlayerScore> playerScore;
        uint updateCounter;

        [BurstCompile]
        public void Execute(Entity asteroidEntity, in LocalToWorld asteroidWorldTransform, in Movement asteroidMovement, in Asteroid asteroid, in Score asteroidScore)
        {
            var random = Random.CreateFromIndex(updateCounter++);

            // Did this projectile hit an asteroid?
            if (math.distancesq(projectileWorldTransform.Position, asteroidWorldTransform.Position) < asteroid.CollisionRadiusSQ)
            {
                // Destroy projectile
                projectileTurret.ValueRW.ProjectilesCurrentlyOnScreen--;
                entityCommandBuffer.DestroyEntity(projectileEntity);

                // Spawn smaller asteroid fragments if linked
                if (asteroid.FragmentsAmount > 0 && asteroid.FragmentPrefab != null)
                {
                    for (int i = 0; i < asteroid.FragmentsAmount; i++)
                    {
                        Entity newAsteroidEntity = entityCommandBuffer.Instantiate(asteroid.FragmentPrefab);
                        entityCommandBuffer.SetComponent(newAsteroidEntity, new LocalTransform
                        {
                            Position = asteroidWorldTransform.Position,
                            Rotation = quaternion.identity,
                            Scale = 1f
                        });
                        entityCommandBuffer.SetComponent(newAsteroidEntity, new Movement
                        {
                            Value = math.normalize(random.NextFloat2(-1f, 1f)) * random.NextFloat(math.length(asteroidMovement.Value) * 1.1f, math.length(asteroidMovement.Value) * 1.5f),
                            Drag = asteroidMovement.Drag,
                            MaxSpeed = asteroidMovement.MaxSpeed
                        });
                    }
                }

                // Add player score
                playerScore.ValueRW.CurrentValue += asteroidScore.Value;

                // Destroy asteroid
                entityCommandBuffer.DestroyEntity(asteroidEntity);
            }
        }
    }
}
