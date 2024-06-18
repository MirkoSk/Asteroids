using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids
{
    public class GameConfigAuthoring : MonoBehaviour
    {
        public Vector2 PlayAreaBounds;

        class Baker : Baker<GameConfigAuthoring>
        {
            public override void Bake(GameConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameConfig
                {
                    PlayAreaBounds = authoring.PlayAreaBounds
                });
            }
        }
    }

    public struct GameConfig : IComponentData
    {
        public float2 PlayAreaBounds;
    }
}

