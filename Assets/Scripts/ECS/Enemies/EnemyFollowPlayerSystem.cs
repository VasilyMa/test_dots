using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]

public partial struct EnemyFollowPlayerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player))
            return;

        float3 playerPos =
            SystemAPI.GetComponent<LocalTransform>(player).Position;

        float dt = SystemAPI.Time.DeltaTime;

        foreach (var (transform, move) in
                 SystemAPI.Query<RefRW<LocalTransform>, MoveComponent>()
                          .WithAll<EnemyComponent>())
        {
            float3 dir = math.normalize(playerPos - transform.ValueRO.Position);

            transform.ValueRW.Position += dir * move.Speed * dt;
        }
    }
}
