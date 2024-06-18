using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class PlayerAuthoring : MonoBehaviour
    {
        public float Force = 0.01f;
        public float RotationSpeed = 1f;

        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Player
                {
                    Force = authoring.Force,
                    RotationSpeed = authoring.RotationSpeed
                });
            }
        }
    }

    struct Player : IComponentData
    {
        public float Force;
        public float RotationSpeed;
    }
}
