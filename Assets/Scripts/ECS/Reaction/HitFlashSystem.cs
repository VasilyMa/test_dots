using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;

[BurstCompile]
public partial struct HitFlashSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (flash, color, baseColor, entity) in SystemAPI.Query<RefRW<HitFlashComponent>, RefRW<URPMaterialPropertyBaseColor>, RefRO<BaseColorComponent>>().WithEntityAccess())
        {
            flash.ValueRW.Timer += dt;
             
            color.ValueRW.Value = new float4(1f, 1f, 1f, 1f);

            if (flash.ValueRW.Timer >= flash.ValueRW.Duration)
            {
                // Возврат исходного цвета
                color.ValueRW.Value = baseColor.ValueRO.Value;

                ecb.RemoveComponent<HitFlashComponent>(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
