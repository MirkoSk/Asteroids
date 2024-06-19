using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class GameConfigAuthoring : MonoBehaviour
    {
        public Vector2 PlayAreaBounds;
        public float ShipRespawnDuration = 3f;

        class Baker : Baker<GameConfigAuthoring>
        {
            public override void Bake(GameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameConfig
                {
                    PlayAreaBounds = authoring.PlayAreaBounds,
                    ShipRespawnDuration = authoring.ShipRespawnDuration
                });
            }
        }
    }

    public struct GameConfig : IComponentData
    {
        public float2 PlayAreaBounds;
        public float ShipRespawnDuration;
    }
}

