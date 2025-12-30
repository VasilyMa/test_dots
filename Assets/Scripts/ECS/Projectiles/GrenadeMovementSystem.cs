using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct GrenadeMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (grenade, transform, entity) in SystemAPI.Query<RefRW<GrenadeComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            // 1️⃣ Продвигаем гранату по времени
            float distance = math.distance(grenade.ValueRO.StartPosition, grenade.ValueRO.TargetPosition);
            float step = grenade.ValueRO.Speed * dt / distance;
            grenade.ValueRW.Progress += step;
            grenade.ValueRW.Progress = math.min(grenade.ValueRW.Progress, 1f);

            // 2️⃣ Вычисляем позицию по дуге
            float3 basePos = math.lerp(grenade.ValueRO.StartPosition, grenade.ValueRO.TargetPosition, grenade.ValueRW.Progress);
            float heightOffset = grenade.ValueRO.ArcHeight * math.sin(math.PI * grenade.ValueRW.Progress);
            basePos.y += heightOffset;
            transform.ValueRW.Position = basePos;

            // 3️⃣ Добавляем вращение
            // Например, вращение вокруг Y и X
            float3 rotationEuler = new float3(360f * grenade.ValueRW.Progress, 720f * grenade.ValueRW.Progress, 0f);
            transform.ValueRW.Rotation = quaternion.EulerXYZ(math.radians(rotationEuler));

            // 4️⃣ Если достигли цели — создаем событие взрыва
            if (grenade.ValueRW.Progress >= 1f)
            {
                Entity explosionEvent = ecb.CreateEntity();
                ecb.AddComponent(explosionEvent, new GrenadeExplosionEvent
                {
                    Position = transform.ValueRO.Position,
                    Radius = grenade.ValueRO.ExplosionRadius,
                    Damage = grenade.ValueRO.Damage
                });

                // Удаляем гранату
                ecb.DestroyEntity(entity);
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
} 