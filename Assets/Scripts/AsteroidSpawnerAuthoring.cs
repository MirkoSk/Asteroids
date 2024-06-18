using NUnit.Framework;
using Unity.Entities;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

namespace Asteroids
{
    class AsteroidSpawnerAuthoring : MonoBehaviour
    {
        public GameObject AsteroidPrefab;
        public int SpawnCount;
        public float Velocity;

        class Baker : Baker<AsteroidSpawnerAuthoring>
        {
            public override void Bake(AsteroidSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AsteroidSpawner
                {
                    AsteroidPrefab = GetEntity(authoring.AsteroidPrefab, TransformUsageFlags.Dynamic),
                    SpawnCount = authoring.SpawnCount,
                    Velocity = authoring.Velocity
                });
            }
        }
    }

    struct AsteroidSpawner : IComponentData
    {
        public Entity AsteroidPrefab;
        public int SpawnCount;
        public float Velocity;
    }
}
