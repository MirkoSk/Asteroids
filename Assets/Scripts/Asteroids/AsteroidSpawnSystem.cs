using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using System.Diagnostics;

namespace Asteroids
{
    partial struct AsteroidSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AsteroidSpawner>();
            state.RequireForUpdate<GameConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // If there are still asteroids on the field => Do nothing
            var asteroidsQuery = SystemAPI.QueryBuilder().WithAll<Asteroid>().Build();
            if (!asteroidsQuery.IsEmpty)
            {
                return;
            }

            var asteroidSpawner = SystemAPI.GetSingletonRW<AsteroidSpawner>();
            var gameConfig = SystemAPI.GetSingleton<GameConfig>();

            // If there are no asteroids on the field => Respawn them delayed and increase the amount (except for the first spawn)
            if (asteroidSpawner.ValueRO.RespawnCounter > 0)
            {
                if (asteroidSpawner.ValueRO.NoAsteroidsTimestamp == 0)
                {
                    asteroidSpawner.ValueRW.NoAsteroidsTimestamp = SystemAPI.Time.ElapsedTime;
                }
                if (SystemAPI.Time.ElapsedTime - asteroidSpawner.ValueRO.NoAsteroidsTimestamp < asteroidSpawner.ValueRO.RespawnDelay)
                {
                    return;
                }
                asteroidSpawner.ValueRW.SpawnCount++;
                asteroidSpawner.ValueRW.NoAsteroidsTimestamp = 0f;
            }

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            var random = Random.CreateFromIndex(asteroidSpawner.ValueRW.RespawnCounter++);

            // TODO: How to spawn a random prefab from an array?
            for (int i = 0; i < asteroidSpawner.ValueRO.SpawnCount; i++)
            {
                var asteroidEntity = entityCommandBuffer.Instantiate(asteroidSpawner.ValueRO.AsteroidPrefab);

                // Set position
                float3 newPosition = float3.zero;
                newPosition.x = random.NextFloat(-gameConfig.PlayAreaBounds.x, gameConfig.PlayAreaBounds.x);
                newPosition.y = random.NextFloat(-gameConfig.PlayAreaBounds.y, gameConfig.PlayAreaBounds.y);
                entityCommandBuffer.SetComponent(asteroidEntity, new LocalTransform
                {
                    Position = newPosition,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                entityCommandBuffer.SetComponent(asteroidEntity, new Movement 
                { 
                    Value = math.normalize(random.NextFloat2(-1f, 1f)) * asteroidSpawner.ValueRO.Velocity * random.NextFloat(0.8f, 1.2f),
                    Drag = SystemAPI.GetComponentRO<Movement>(asteroidSpawner.ValueRO.AsteroidPrefab).ValueRO.Drag,
                    MaxSpeed = SystemAPI.GetComponentRO<Movement>(asteroidSpawner.ValueRO.AsteroidPrefab).ValueRO.MaxSpeed
                });
            }

            entityCommandBuffer.Playback(state.EntityManager);

            entityCommandBuffer.Dispose();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
