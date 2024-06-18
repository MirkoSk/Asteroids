using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateBefore(typeof(MovementSystem))]
partial struct ShipMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float2 inputDirection;
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.y = Input.GetAxis("Vertical");

        if (!inputDirection.Equals(float2.zero))
        {
            foreach (var (movement, player) in SystemAPI.Query<RefRW<Movement>, RefRO<Player>>())
            {
                movement.ValueRW.Value += inputDirection * player.ValueRO.Force;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
