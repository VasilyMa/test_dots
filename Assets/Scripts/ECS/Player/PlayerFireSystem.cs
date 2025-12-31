/*using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct PlayerFireSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        _random = new Random(12345);
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var em = state.EntityManager;

        // Игрок
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
            return;

        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);

        foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyComponent>().WithEntityAccess())
        {
            enemies.Add(entity);
        }

        if (enemies.Length == 0)
        {
            enemies.Dispose();
            return;
        }
         
        foreach (var weapon in SystemAPI.Query<RefRW<WeaponComponent>>())
        {
            weapon.ValueRW.Delay -= dt;
            if (weapon.ValueRW.Delay > 0f)
                continue;

            weapon.ValueRW.Delay = weapon.ValueRO.Firetick;

            int shots = math.min(weapon.ValueRO.BulletsPerShot, enemies.Length);

            for (int i = 0; i < shots; i++)
            {
                int index = _random.NextInt(0, enemies.Length);
                Entity target = enemies[index];

                float3 targetPos = em.GetComponentData<LocalTransform>(target).Position;

                Entity bullet = em.Instantiate(weapon.ValueRO.BulletPrefab);

                var bulletData = em.GetComponentData<BulletComponent>(bullet);
                bulletData.Direction = math.normalize(targetPos - playerPos);
                bulletData.Target = target;
                em.SetComponentData(bullet, bulletData);

                var bulletTransform = em.GetComponentData<LocalTransform>(bullet);
                bulletTransform.Position = playerPos;
                bulletTransform.Rotation = quaternion.identity; 
                em.SetComponentData(bullet, bulletTransform);

                // Убираем цель, чтобы не стреляли все в одну
                enemies.RemoveAtSwapBack(index);
                if (enemies.Length == 0)
                    break;
            }
        }
         
        foreach (var launcher in SystemAPI.Query<RefRW<LauncherComponent>>())
        {
            launcher.ValueRW.Delay -= dt;
            if (launcher.ValueRW.Delay > 0f)
                continue;

            launcher.ValueRW.Delay = launcher.ValueRO.Firetick;

            int shots = math.min(launcher.ValueRO.GrenadesPerShot, enemies.Length);

            for (int i = 0; i < shots; i++)
            {
                int index = _random.NextInt(0, enemies.Length);
                Entity target = enemies[index];

                float3 targetPos = em.GetComponentData<LocalTransform>(target).Position;

                Entity grenade = em.Instantiate(launcher.ValueRO.GrenadePrefab);

                var grenadeData = em.GetComponentData<GrenadeComponent>(grenade);
                grenadeData.StartPosition = playerPos;
                grenadeData.TargetPosition = targetPos;
                grenadeData.Progress = 0f;
                em.SetComponentData(grenade, grenadeData);

                var grenadeTransform = em.GetComponentData<LocalTransform>(grenade);
                grenadeTransform.Position = playerPos;
                grenadeTransform.Rotation = quaternion.identity; 
                em.SetComponentData(grenade, grenadeTransform);

                enemies.RemoveAtSwapBack(index);
                if (enemies.Length == 0)
                    break;
            }
        }

        enemies.Dispose();
    }
} */