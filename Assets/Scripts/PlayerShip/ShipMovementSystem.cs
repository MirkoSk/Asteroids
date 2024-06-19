using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(MovementSystem))]
[BurstCompile]
partial class ShipMovementSystem : SystemBase
{
    public event System.Action<bool> OnThrust;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<Ship>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        float turnInput = Input.GetAxis("Horizontal");
        float accelerateInput = math.clamp(Input.GetAxis("Vertical"), 0f, 1f);

        // Received any input?
        if (accelerateInput != 0f || turnInput != 0f)
        {
            foreach (var (transform, movement, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Movement>, RefRO<Ship>>())
            {
                // Rotate the ship
                transform.ValueRW = transform.ValueRO.RotateZ(-turnInput * player.ValueRO.RotationSpeed * SystemAPI.Time.DeltaTime);

                // Update the movement component depending on input
                float2 moveDirection = new float2(transform.ValueRO.Up().x, transform.ValueRO.Up().y);
                movement.ValueRW.Value += moveDirection * accelerateInput * player.ValueRO.Force * SystemAPI.Time.DeltaTime;
            }
        }

        // Update ship's thruster graphic depending on move input
        // Remark: Means a structural change but should be fine for once per frame. Maybe there's a more efficient way?
        foreach (var (shipThruster, entity) in SystemAPI.Query<RefRO<ShipThruster>>().WithEntityAccess().WithOptions(EntityQueryOptions.IncludeDisabledEntities))
        {
            if (accelerateInput != 0 && SystemAPI.HasComponent<Disabled>(entity))
            {
                entityCommandBuffer.RemoveComponent<Disabled>(entity);
                OnThrust?.Invoke(true);
            }
            else if (accelerateInput == 0 && !SystemAPI.HasComponent<Disabled>(entity))
            {
                entityCommandBuffer.AddComponent<Disabled>(entity);
                OnThrust?.Invoke(false);
            }
        }

        entityCommandBuffer.Playback(EntityManager);

        entityCommandBuffer.Dispose();
    }
}
