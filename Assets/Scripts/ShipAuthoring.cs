using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class ShipAuthoring : MonoBehaviour
    {
        public float Force = 0.01f;
        public float RotationSpeed = 1f;

        class Baker : Baker<ShipAuthoring>
        {
            public override void Bake(ShipAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Ship
                {
                    Force = authoring.Force,
                    RotationSpeed = authoring.RotationSpeed
                });
            }
        }
    }

    struct Ship : IComponentData
    {
        public float Force;
        public float RotationSpeed;
    }
}
