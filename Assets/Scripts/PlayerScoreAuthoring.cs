using System;
using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class PlayerScoreAuthoring : MonoBehaviour
    {
        public int CurrentValue;

        class Baker : Baker<PlayerScoreAuthoring>
        {
            public override void Bake(PlayerScoreAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerScore
                {
                    CurrentValue = authoring.CurrentValue
                });
            }
        }
    }

    public struct PlayerScore : IComponentData
    {
        public int CurrentValue;
        public int ValueLastFrame;
    }
}

