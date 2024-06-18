using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class PlayerAuthoring : MonoBehaviour
    {
        public float Force;

        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Player
                {
                    Force = authoring.Force
                });
            }
        }
    }

    struct Player : IComponentData
    {
        public float Force;
    }
}
