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
                Entity projectile = state.EntityManager.Instantiate(turret.ValueRO.ProjectilePrefab);

                // Set transform
                var bulletTransform = SystemAPI.GetComponentRW<LocalTransform>(projectile);
                bulletTransform.ValueRW.Position = transform.ValueRO.Position;
                bulletTransform.ValueRW.Rotation = transform.ValueRO.Rotation;

                // Set movement
                float2 shootingDirection = new float2(transform.ValueRO.Up.x, transform.ValueRO.Up.y);
                var bulletMovement = SystemAPI.GetComponentRW<Movement>(projectile);
                bulletMovement.ValueRW.Value = math.normalize(shootingDirection) * turret.ValueRO.ProjectileSpeed;
                bulletMovement.ValueRW.MaxSpeed = turret.ValueRO.ProjectileSpeed;

                // Set projectile lifetime
                var projectileComponent = SystemAPI.GetComponentRW<Projectile>(projectile);
                projectileComponent.ValueRW.SpawnTime = SystemAPI.Time.ElapsedTime;
                projectileComponent.ValueRW.Lifetime = turret.ValueRO.ProjectileLifetime;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
