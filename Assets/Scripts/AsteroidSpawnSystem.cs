using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

namespace Asteroids
{
    partial struct AsteroidSpawnSystem : ISystem
    {
        uint updateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AsteroidSpawner>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var asteroidSpawner = SystemAPI.GetSingleton<AsteroidSpawner>();
            var gameConfig = SystemAPI.GetSingleton<GameConfig>();

            var random = Random.CreateFromIndex(updateCounter++);

            // TODO: How to spawn a random prefab from an array?
            var instances = state.EntityManager.Instantiate(asteroidSpawner.AsteroidPrefab, asteroidSpawner.SpawnCount, Allocator.Temp);

            foreach (var asteroid in instances)
            {
                var transform = SystemAPI.GetComponentRW<LocalTransform>(asteroid);
                float3 newPosition = float3.zero;
                newPosition.x = random.NextFloat(-gameConfig.PlayAreaBounds.x, gameConfig.PlayAreaBounds.x);
                newPosition.y = random.NextFloat(-gameConfig.PlayAreaBounds.y, gameConfig.PlayAreaBounds.y);
                transform.ValueRW.Position = newPosition;

                var movement = SystemAPI.GetComponentRW<Movement>(asteroid);
                movement.ValueRW.Value = math.normalize(random.NextFloat2(-1f, 1f)) * asteroidSpawner.Velocity;
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}
