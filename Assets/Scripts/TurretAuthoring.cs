using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class TurretAuthoring : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public float ProjectileSpeed = 10f;
        public float ProjectileLifetime = 1f;
        public int MaxProjectilesOnScreen = 4;

        class Baker : Baker<TurretAuthoring>
        {
            public override void Bake(TurretAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Turret
                {
                    ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic),
                    ProjectileSpeed = authoring.ProjectileSpeed,
                    ProjectileLifetime = authoring.ProjectileLifetime,
                    MaxProjectilesOnScreen = authoring.MaxProjectilesOnScreen
                });
            }
        }
    }

    struct Turret : IComponentData
    {
        public Entity ProjectilePrefab;
        public float ProjectileSpeed;
        public double ProjectileLifetime;
        public int MaxProjectilesOnScreen;
        public int ProjectilesCurrentlyOnScreen;
    }
}
