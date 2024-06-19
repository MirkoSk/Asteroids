using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(MovementSystem))]
partial struct ShipMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ship>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        float turnInput = Input.GetAxis("Horizontal");
        float accelerateInput = math.clamp(Input.GetAxis("Vertical"), 0f, 1f);

        if (accelerateInput != 0f || turnInput != 0f)
        {
            foreach (var (transform, movement, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Movement>, RefRO<Ship>>())
            {
                // Rotate the ship
                transform.ValueRW = transform.ValueRO.RotateZ(-turnInput * player.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime);

                // Update the movement component depending on input
                float2 moveDirection = new float2(transform.ValueRO.Up().x, transform.ValueRO.Up().y);
                movement.ValueRW.Value += moveDirection * accelerateInput * player.ValueRO.Force;
            }
        }

        // Update ship's thruster graphic depending on move input
        foreach (var (shipThruster, entity) in SystemAPI.Query<RefRO<ShipThruster>>().WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
        {
            if (accelerateInput != 0 && SystemAPI.HasComponent<Disabled>(entity))
            {
                entityCommandBuffer.RemoveComponent<Disabled>(entity);
            }
            else if (accelerateInput == 0 && !SystemAPI.HasComponent<Disabled>(entity))
            {
                entityCommandBuffer.AddComponent<Disabled>(entity);
            }
        }

        entityCommandBuffer.Playback(state.EntityManager);

        entityCommandBuffer.Dispose();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
