using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    class ShipThrusterAuthoring : MonoBehaviour
    {

        class Baker : Baker<ShipThrusterAuthoring>
        {
            public override void Bake(ShipThrusterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShipThruster
                {

                });
            }
        }
    }

    struct ShipThruster : IComponentData
    {

    }
}
