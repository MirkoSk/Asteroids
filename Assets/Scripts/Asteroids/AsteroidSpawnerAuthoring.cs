using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class AsteroidSpawnerAuthoring : MonoBehaviour
    {
        public GameObject AsteroidPrefab;
        public int SpawnCount;
        public float Velocity;
        public float RespawnDelay;

        class Baker : Baker<AsteroidSpawnerAuthoring>
        {
            public override void Bake(AsteroidSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AsteroidSpawner
                {
                    AsteroidPrefab = GetEntity(authoring.AsteroidPrefab, TransformUsageFlags.Dynamic),
                    SpawnCount = authoring.SpawnCount,
                    InitialSpawnCount = authoring.SpawnCount,
                    Velocity = authoring.Velocity,
                    RespawnDelay = authoring.RespawnDelay
                });
            }
        }
    }

    struct AsteroidSpawner : IComponentData
    {
        public Entity AsteroidPrefab;
        public int SpawnCount;
        public int InitialSpawnCount;
        public float Velocity;
        public double NoAsteroidsTimestamp;
        public double RespawnDelay;
        public uint RespawnCounter;
    }
}
