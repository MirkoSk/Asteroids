using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct OutOfBoundsSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Movement>();
        state.RequireForUpdate<GameConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        var job = new OutOfBoundsJob
        {
            gameConfig = gameConfig
        };
        job.Schedule();
    }

    [BurstCompile]
    public partial struct OutOfBoundsJob : IJobEntity
    {
        public GameConfig gameConfig;

        public void Execute(ref LocalTransform transform)
        {
            if (math.abs(transform.Position.x) > gameConfig.PlayAreaBounds.x)
            {
                transform.Position = new float3(-transform.Position.x, transform.Position.y, transform.Position.z);
            }
            else if (math.abs(transform.Position.y) > gameConfig.PlayAreaBounds.y)
            {
                transform.Position = new float3(transform.Position.x, -transform.Position.y, transform.Position.z);
            }
        }
    }
}
