using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class ShootingSystem : SystemBase
{
    public event System.Action OnShoot;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<Turret>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (Input.GetButtonDown("Fire"))
        {
            foreach (var (transform, turret, turretEntity) in SystemAPI.Query<RefRO<LocalToWorld>, RefRW<Turret>>().WithEntityAccess())
            {
                // Only x projectiles allowed on screen at the same time
                if (turret.ValueRO.ProjectilesCurrentlyOnScreen >= turret.ValueRO.MaxProjectilesOnScreen)
                {
                    return;
                }

                Entity projectile = EntityManager.Instantiate(turret.ValueRO.ProjectilePrefab);

                // Set transform
                var bulletTransform = SystemAPI.GetComponentRW<LocalTransform>(projectile);
                bulletTransform.ValueRW.Position = transform.ValueRO.Position;
                bulletTransform.ValueRW.Rotation = transform.ValueRO.Rotation;

                // Set movement
                float2 shootingDirection = new float2(transform.ValueRO.Up.x, transform.ValueRO.Up.y);
                var bulletMovement = SystemAPI.GetComponentRW<Movement>(projectile);
                bulletMovement.ValueRW.Value = math.normalize(shootingDirection) * turret.ValueRO.ProjectileSpeed;
                bulletMovement.ValueRW.MaxSpeed = turret.ValueRO.ProjectileSpeed;

                // Set projectile lifetime and turret reference
                var projectileComponent = SystemAPI.GetComponentRW<Projectile>(projectile);
                projectileComponent.ValueRW.SpawnTime = SystemAPI.Time.ElapsedTime;
                projectileComponent.ValueRW.Lifetime = turret.ValueRO.ProjectileLifetime;
                projectileComponent.ValueRW.Turret = turretEntity;

                // Inform GameObject land
                OnShoot?.Invoke();

                turret.ValueRW.ProjectilesCurrentlyOnScreen++;
            }
        }
    }
}
