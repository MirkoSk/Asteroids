using Unity.Entities;
using UnityEngine;

namespace Asteroids
{
    public class ScoreAuthoring : MonoBehaviour
    {
        public int Value;

        class Baker : Baker<ScoreAuthoring>
        {
            public override void Bake(ScoreAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Score
                {
                    Value = authoring.Value
                });
            }
        }
    }

    public struct Score : IComponentData
    {
        public int Value;
    }
}

