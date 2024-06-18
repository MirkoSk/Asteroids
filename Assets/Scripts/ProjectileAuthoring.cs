using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class ProjectileAuthoring : MonoBehaviour
    {
        class Baker : Baker<ProjectileAuthoring>
        {
            public override void Bake(ProjectileAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Projectile
                {

                });
            }
        }
    }

    struct Projectile : IComponentData
    {
        public double SpawnTime;
        public double Lifetime;
    }
}
