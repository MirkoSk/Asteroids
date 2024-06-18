using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class AsteroidAuthoring : MonoBehaviour
    {
        public float CollisionRadius = 1f;
        public GameObject FragmentPrefab;
        public int FragmentsAmount = 2;

        class Baker : Baker<AsteroidAuthoring>
        {
            public override void Bake(AsteroidAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Asteroid
                {
                    FragmentPrefab = GetEntity(authoring.FragmentPrefab, TransformUsageFlags.Dynamic),
                    FragmentsAmount = authoring.FragmentsAmount,
                    CollisionRadiusSQ = authoring.CollisionRadius * authoring.CollisionRadius
                });
            }
        }
    }

    struct Asteroid : IComponentData
    {
        public float CollisionRadiusSQ;
        public Entity FragmentPrefab;
        public int FragmentsAmount;
    }
}
