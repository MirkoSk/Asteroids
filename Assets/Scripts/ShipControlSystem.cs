using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(MovementSystem))]
partial struct ShipControlSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float turnInput = Input.GetAxis("Horizontal");
        float accelerateInput = math.clamp(Input.GetAxis("Vertical"), 0f, 1f);

        if (accelerateInput != 0f || !Equals(turnInput, float2.zero))
        {
            foreach (var (transform, movement, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Movement>, RefRO<Player>>())
            {
                // Rotate the ship
                transform.ValueRW = transform.ValueRO.RotateZ(-turnInput * player.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime);

                // Update the movement component depending on input
                float2 moveDirection = new float2(transform.ValueRO.Up().x, transform.ValueRO.Up().y);
                movement.ValueRW.Value += moveDirection * accelerateInput * player.ValueRO.Force;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
