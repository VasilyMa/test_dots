using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{ 
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (input, move, transform) in
            SystemAPI.Query
                <RefRO<InputComponent>,
                RefRO<MoveComponent>,
                RefRW<LocalTransform>>()
                .WithAll<PlayerComponent>())
        {
            if (math.lengthsq(input.ValueRO.Move) < 0.0001f)
                continue;

            float3 dir = math.normalize(new float3(input.ValueRO.Move.x, 0f, input.ValueRO.Move.y));

            transform.ValueRW.Position += dir * move.ValueRO.Speed * dt;
            transform.ValueRW.Rotation = quaternion.LookRotationSafe(dir, math.up());
        }
    }
}