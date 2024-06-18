using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct ShootingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Turret>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetButtonDown("Fire"))
        {
            foreach (var (transform, turret) in SystemAPI.Query<RefRO<LocalToWorld>, RefRO<Turret>>())
            {
                Entity bullet = state.EntityManager.Instantiate(turret.ValueRO.ProjectilePrefab);

                // Set transform
                var bulletTransform = SystemAPI.GetComponentRW<LocalTransform>(bullet);
                bulletTransform.ValueRW.Position = transform.ValueRO.Position;
                bulletTransform.ValueRW.Rotation = transform.ValueRO.Rotation;

                // Set movement
                float2 shootingDirection = new float2(transform.ValueRO.Up.x, transform.ValueRO.Up.y);
                var bulletMovement = SystemAPI.GetComponentRW<Movement>(bullet);
                bulletMovement.ValueRW.Value = math.normalize(shootingDirection) * turret.ValueRO.ProjectileSpeed;
                bulletMovement.ValueRW.MaxSpeed = turret.ValueRO.ProjectileSpeed;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
