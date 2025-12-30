using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct BulletMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (bullet, transform, entity) in SystemAPI.Query<RefRW<BulletComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            // Уменьшаем жизнь пули
            bullet.ValueRW.Lifetime -= dt;
            if (bullet.ValueRO.Lifetime <= 0f)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            // Определяем направление
            float3 dir = bullet.ValueRO.Direction;
            if (bullet.ValueRO.Target != Entity.Null && SystemAPI.HasComponent<LocalTransform>(bullet.ValueRO.Target))
            {
                float3 targetPos = SystemAPI.GetComponent<LocalTransform>(bullet.ValueRO.Target).Position;
                dir = math.normalize(targetPos - transform.ValueRO.Position);
                bullet.ValueRW.Direction = dir;
            }

            // Движение пули
            transform.ValueRW.Position += dir * bullet.ValueRO.Speed * dt;

            // Проверка попадания
            if (bullet.ValueRO.Target != Entity.Null &&
                SystemAPI.HasComponent<LocalTransform>(bullet.ValueRO.Target))
            {
                float3 targetPos = SystemAPI.GetComponent<LocalTransform>(bullet.ValueRO.Target).Position;
                float distSq = math.lengthsq(targetPos - transform.ValueRO.Position);

                if (distSq < 0.25f) // радиус попадания
                {
                    if (SystemAPI.HasComponent<HealthComponent>(bullet.ValueRO.Target))
                    {
                        var health = SystemAPI.GetComponent<HealthComponent>(bullet.ValueRO.Target);
                        health.CurrentHealth -= bullet.ValueRO.Damage;
                        ecb.SetComponent(bullet.ValueRO.Target, health);

                        // Добавляем Knockback
                        var knockback = new KnockbackComponent
                        {
                            Direction = math.normalize(targetPos - transform.ValueRO.Position), // отталкивание от точки попадания
                            Force = 2f,
                            Duration = 0.2f,
                            Timer = 0f
                        };
                        ecb.AddComponent(bullet.ValueRO.Target, knockback);

                        // Добавляем HitFlash
                        var hitFlash = new HitFlashComponent
                        {
                            Duration = 0.1f,
                            Timer = 0f
                        };
                        ecb.AddComponent(bullet.ValueRO.Target, hitFlash);
                    }

                    // Уничтожаем пулю
                    ecb.DestroyEntity(entity);
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
