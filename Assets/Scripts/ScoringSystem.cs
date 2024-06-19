using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System;

[BurstCompile]
partial class ScoringSystem : SystemBase
{
    public event Action<int> OnScoring;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerScore>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var playerScore = SystemAPI.GetSingleton<PlayerScore>();

        if (playerScore.CurrentValue != playerScore.ValueLastFrame)
        {
            playerScore.ValueLastFrame = playerScore.CurrentValue;
            OnScoring?.Invoke(playerScore.CurrentValue);
        }
    }

    public void ResetScore()
    {
        var playerScore = SystemAPI.GetSingleton<PlayerScore>();
        playerScore.CurrentValue = 0;
    }
}