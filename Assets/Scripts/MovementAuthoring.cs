using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    class MovementAuthoring : MonoBehaviour
    {
        public float Drag;
        public float MaxSpeed = 10f;

        class Baker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Movement
                {
                    Drag = authoring.Drag,
                    MaxSpeed = authoring.MaxSpeed
                });
            }
        }
    }

    struct Movement : IComponentData
    {
        public float2 Value;
        public float Drag;
        public float MaxSpeed;
    }
}
