using Asteroids;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Movement>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new MovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        job.Schedule();
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref LocalTransform transform, ref Movement movement)
        {
            // Factor in drag
            movement.Value *= (1 - deltaTime * movement.Drag);

            // Clamp vector to MaxSpeed
            if (math.length(movement.Value) > movement.MaxSpeed) movement.Value = math.normalize(movement.Value) * movement.MaxSpeed;

            // Update Transform
            transform = transform.Translate(new float3(movement.Value.x, movement.Value.y, 0f) * deltaTime);
        }
    }
}
