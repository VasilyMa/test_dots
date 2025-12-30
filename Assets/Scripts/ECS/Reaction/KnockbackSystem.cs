using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

[BurstCompile]
public partial struct KnockbackSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (knockback, transform, entity) in
                 SystemAPI.Query<RefRW<KnockbackComponent>, RefRW<LocalTransform>>()
                          .WithEntityAccess())
        {
            // Движение врага
            transform.ValueRW.Position += knockback.ValueRO.Direction * knockback.ValueRO.Force * dt;

            // Таймер
            knockback.ValueRW.Timer += dt;

            if (knockback.ValueRO.Timer >= knockback.ValueRO.Duration)
            {
                // Добавляем удаление через ECB
                ecb.RemoveComponent<KnockbackComponent>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
