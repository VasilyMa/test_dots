using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Burst;

[BurstCompile]
public partial struct ExplosionVisualSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (explode, transform, color, entity) in
          SystemAPI.Query<RefRW<ExplodeComponent>, RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>>()
                   .WithEntityAccess())
        {
            explode.ValueRW.Timer += dt;
            float progress = math.min(explode.ValueRW.Timer / explode.ValueRW.Duration, 1f);

            // Масштаб: быстро растёт, потом затухает
            float scale = explode.ValueRW.MaxRadius * math.sin(progress * math.PI);
            transform.ValueRW.Scale = scale;

            // Прозрачность: плавно затухает
            var col = color.ValueRW.Value;
            col.w = 1f - progress;  // а это alpha
            color.ValueRW.Value = col; // присваиваем обратно

            // Удаляем после окончания анимации
            if (explode.ValueRW.Timer >= explode.ValueRW.Duration)
            {
                ecb.DestroyEntity(entity);
            }
        }


        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
