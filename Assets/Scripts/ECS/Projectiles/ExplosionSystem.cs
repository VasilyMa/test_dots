using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms; 

[BurstCompile]
public partial struct ExplosionSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (explosionEvent, entity) in  SystemAPI.Query<RefRO<GrenadeExplosionEvent>>().WithEntityAccess())
        {
            // 1️⃣ Наносим урон всем врагам в радиусе
            float3 pos = explosionEvent.ValueRO.Position;
            float radius = explosionEvent.ValueRO.Radius;
            float damage = explosionEvent.ValueRO.Damage;

            foreach (var (enemy, enemyTransform, enemyEntity) in SystemAPI.Query<RefRW<HealthComponent>, RefRO<LocalTransform>>().WithAll<EnemyComponent>().WithEntityAccess())
            {
                float3 enemyPos = enemyTransform.ValueRO.Position;
                float distSq = math.distancesq(pos, enemyPos);

                if (distSq <= radius * radius)
                {
                    enemy.ValueRW.CurrentHealth -= damage;
                     
                    if (!SystemAPI.HasComponent<HitFlashComponent>(enemyEntity))
                        ecb.AddComponent(enemyEntity, new HitFlashComponent { Timer = 0f, Duration = 0.2f });

                    if (!SystemAPI.HasComponent<KnockbackComponent>(enemyEntity))
                        ecb.AddComponent(enemyEntity, new KnockbackComponent
                        {
                            Timer = 0f,
                            Duration = 0.3f,
                            Force = 5f,
                            Direction = math.normalize(pos - enemyPos)
                        });
                }
            }

            var entityManager = state.EntityManager;

            foreach (var exploder in SystemAPI.Query<RefRW<ExploderComponent>>())
            {
                Entity explodeEntity = entityManager.Instantiate(exploder.ValueRO.Explode); 

                var explodeTransform = entityManager.GetComponentData<LocalTransform>(explodeEntity);
                explodeTransform.Position = pos;
                explodeTransform.Rotation = quaternion.identity;
                explodeTransform.Scale = 0f; 

                entityManager.SetComponentData(explodeEntity, explodeTransform);
            } 

            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
