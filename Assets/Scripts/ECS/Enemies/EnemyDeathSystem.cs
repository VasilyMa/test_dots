using Unity.Entities;
using Unity.Burst;

[BurstCompile]
public partial struct EnemyDeathSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (health, entity) in
                 SystemAPI.Query<RefRO<HealthComponent>>()
                     .WithAll<EnemyComponent>()
                     .WithEntityAccess())
        {
            if (health.ValueRO.CurrentHealth <= 0f)
            {
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
