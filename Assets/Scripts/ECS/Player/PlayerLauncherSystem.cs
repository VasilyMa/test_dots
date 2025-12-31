using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerLauncherSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        _random = new Random(54321); // другой сид для разнообразия
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var em = state.EntityManager;

        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
            return;

        float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

        foreach (var launcher in SystemAPI.Query<RefRW<LauncherComponent>>())
        {
            launcher.ValueRW.Delay -= dt;
            if (launcher.ValueRW.Delay > 0f)
                continue;

            launcher.ValueRW.Delay = launcher.ValueRO.Firetick;

            // Собираем врагов только на момент выстрела
            NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);
            foreach (var (enemyTransform, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyComponent>().WithEntityAccess())
                enemies.Add(enemyEntity);

            if (enemies.Length == 0)
            {
                enemies.Dispose();
                continue;
            }

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
                if (enemies.Length == 0) break;
            }

            enemies.Dispose();
        }
    }
}