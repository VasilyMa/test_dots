using Unity.Entities;
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

        // Получаем игрока
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
            return;
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // Собираем врагов
        NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);
        NativeList<float> weights = new NativeList<float>(Allocator.Temp);
        float totalWeight = 0f;

        foreach (var (enemyTransform, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<EnemyComponent>().WithEntityAccess())
        {
            enemies.Add(enemyEntity);
            float dist = math.distance(playerTransform.Position, enemyTransform.ValueRO.Position);
            float weight = 1f / math.max(dist, 0.1f);
            weights.Add(weight);
            totalWeight += weight;
        }

        if (enemies.Length == 0)
        {
            enemies.Dispose();
            weights.Dispose();
            return;
        }

        // ----------- Стрельба обычным оружием -----------
        foreach (var weapon in SystemAPI.Query<RefRW<WeaponComponent>>())
        {
            weapon.ValueRW.Delay -= dt;
            if (weapon.ValueRW.Delay > 0f) continue;
            weapon.ValueRW.Delay = weapon.ValueRO.Firetick;

            // Создаем копию списка врагов и весов для каждого выстрела
            NativeList<Entity> availableEnemies = new NativeList<Entity>(Allocator.Temp);
            NativeList<float> availableWeights = new NativeList<float>(Allocator.Temp);
            availableEnemies.CopyFrom(enemies);
            availableWeights.CopyFrom(weights);
            float availableTotalWeight = totalWeight;

            for (int i = 0; i < weapon.ValueRO.BulletsPerShot; i++)
            {
                if (availableEnemies.Length == 0) break;

                // Weighted random выбор цели
                float r = _random.NextFloat(0, availableTotalWeight);
                Entity target = Entity.Null;
                int targetIndex = -1;

                for (int j = 0; j < availableEnemies.Length; j++)
                {
                    r -= availableWeights[j];
                    if (r <= 0f)
                    {
                        target = availableEnemies[j];
                        targetIndex = j;
                        break;
                    }
                }

                if (target == Entity.Null) continue;

                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);

                // Создаем пулю
                Entity bullet = em.Instantiate(weapon.ValueRO.BulletPrefab);
                var bulletData = em.GetComponentData<BulletComponent>(bullet);
                bulletData.Direction = math.normalize(targetTransform.Position - playerTransform.Position);
                bulletData.Target = target; 
                em.SetComponentData(bullet, bulletData);

                var bulletTransform = em.GetComponentData<LocalTransform>(bullet);
                bulletTransform.Position = playerTransform.Position;
                bulletTransform.Rotation = quaternion.identity;
                em.SetComponentData(bullet, bulletTransform);

                // Убираем выбранную цель из доступных (если больше одной)
                if (availableEnemies.Length > 1)
                {
                    availableTotalWeight -= availableWeights[targetIndex];
                    availableEnemies.RemoveAt(targetIndex);
                    availableWeights.RemoveAt(targetIndex);
                }
            }

            availableEnemies.Dispose();
            availableWeights.Dispose();
        }

        // ----------- Стрельба гранатометом -----------
        foreach (var launcher in SystemAPI.Query<RefRW<LauncherComponent>>())
        {
            launcher.ValueRW.Delay -= dt;
            if (launcher.ValueRW.Delay > 0f) continue;
            launcher.ValueRW.Delay = launcher.ValueRO.Firetick;

            NativeList<Entity> availableEnemies = new NativeList<Entity>(Allocator.Temp);
            availableEnemies.CopyFrom(enemies);

            for (int i = 0; i < launcher.ValueRO.GrenadesPerShot; i++)
            {
                if (availableEnemies.Length == 0) break;

                // Weighted random для гранаты (по расстоянию)
                NativeList<float> grenadeWeights = new NativeList<float>(Allocator.Temp);
                float total = 0f;
                for (int j = 0; j < availableEnemies.Length; j++)
                {
                    float dist = math.distance(playerTransform.Position, SystemAPI.GetComponent<LocalTransform>(availableEnemies[j]).Position);
                    float w = 1f / math.max(dist, 0.1f);
                    grenadeWeights.Add(w);
                    total += w;
                }

                float r = _random.NextFloat(0, total);
                Entity target = Entity.Null;
                int targetIndex = -1;

                for (int j = 0; j < availableEnemies.Length; j++)
                {
                    r -= grenadeWeights[j];
                    if (r <= 0f)
                    {
                        target = availableEnemies[j];
                        targetIndex = j;
                        break;
                    }
                }

                grenadeWeights.Dispose();
                if (target == Entity.Null) continue;

                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);

                // Создаем гранату
                Entity grenade = em.Instantiate(launcher.ValueRO.GrenadePrefab);
                var grenadeData = em.GetComponentData<GrenadeComponent>(grenade);
                grenadeData.StartPosition = playerTransform.Position;
                grenadeData.TargetPosition = targetTransform.Position;
                grenadeData.Progress = 0f; 
                em.SetComponentData(grenade, grenadeData);

                var grenadeTransform = em.GetComponentData<LocalTransform>(grenade);
                grenadeTransform.Position = playerTransform.Position;
                grenadeTransform.Rotation = quaternion.identity;
                em.SetComponentData(grenade, grenadeTransform);

                if (availableEnemies.Length > 1)
                    availableEnemies.RemoveAt(targetIndex);
            }

            availableEnemies.Dispose();
        }

        enemies.Dispose();
        weights.Dispose();
    }
}

/*using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;
using UnityEngine;
[BurstCompile]
public partial struct PlayerFireSystem : ISystem
{
    private Unity.Mathematics.Random _random;

    public void OnCreate(ref SystemState state)
    {
        _random = new Unity.Mathematics.Random(12345);
    }

    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;
        var entityManager = state.EntityManager;

        // 1️⃣ Получаем игрока (Singleton)
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
            return;

        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // 2️⃣ Собираем врагов (без аллокаций)
        NativeList<Entity> enemies = new NativeList<Entity>(Allocator.Temp);
        NativeList<float> weights = new NativeList<float>(Allocator.Temp);

        float totalWeight = 0f;

        foreach (var (enemyTransform, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>>() .WithAll<EnemyComponent>() .WithEntityAccess())
        {
            float dist = math.distance(playerTransform.Position, enemyTransform.ValueRO.Position);
            float weight = 1f / math.max(dist, 0.1f);

            enemies.Add(enemyEntity);
            weights.Add(weight);
            totalWeight += weight;
        }

        if (enemies.Length == 0)
        {
            enemies.Dispose();
            weights.Dispose();
            return;
        }

        // 3️⃣ Weighted random
        float r = _random.NextFloat(0, totalWeight);
        Entity target = Entity.Null;

        for (int i = 0; i < enemies.Length; i++)
        {
            r -= weights[i];
            if (r <= 0f)
            {
                target = enemies[i];
                break;
            }
        }

        enemies.Dispose();
        weights.Dispose();

        if (target == Entity.Null)
            return;

        if (!SystemAPI.HasComponent<LocalTransform>(target))
            return;

        var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);
         
        foreach (var weapon in SystemAPI.Query<RefRW<WeaponComponent>>())
        {
            weapon.ValueRW.Delay -= dt;

            if (weapon.ValueRW.Delay > 0f)
                continue;

            weapon.ValueRW.Delay = weapon.ValueRO.Firetick;

            Entity bullet = entityManager.Instantiate(weapon.ValueRO.BulletPrefab);

            var bulletData = SystemAPI.GetComponent<BulletComponent>(bullet);

            float3 dir = math.normalize(targetTransform.Position - playerTransform.Position);

            bulletData.Direction = dir;
            bulletData.Target = target;  

            entityManager.SetComponentData(bullet, bulletData);

            var bulletTransform = SystemAPI.GetComponent<LocalTransform>(bullet);
            bulletTransform.Position = playerTransform.Position;
            bulletTransform.Rotation = quaternion.identity;
            entityManager.SetComponentData(bullet, bulletTransform);
        }

        foreach (var launcher in SystemAPI.Query<RefRW<LauncherComponent>>())
        {
            launcher.ValueRW.Delay -= dt;
            if (launcher.ValueRW.Delay > 0f)
                continue;

            launcher.ValueRW.Delay = launcher.ValueRO.Firetick;

            // Создаем гранату
            Entity grenade = entityManager.Instantiate(launcher.ValueRO.GrenadePrefab);

            // Получаем компонент гранаты
            var grenadeData = entityManager.GetComponentData<GrenadeComponent>(grenade);

            // Задаем стартовую позицию (позиция игрока)
            grenadeData.StartPosition = playerTransform.Position;

            // Задаем цель (позиция выбранного врага)
            float3 targetPos = entityManager.GetComponentData<LocalTransform>(target).Position;
            grenadeData.TargetPosition = targetPos;

            // Настройки движения
            grenadeData.Progress = 0f;  

            // Применяем данные к сущности
            entityManager.SetComponentData(grenade, grenadeData);

            // Устанавливаем стартовый трансформ
            var grenadeTransform = entityManager.GetComponentData<LocalTransform>(grenade);
            grenadeTransform.Position = playerTransform.Position;
            grenadeTransform.Rotation = quaternion.identity;
            entityManager.SetComponentData(grenade, grenadeTransform);
        }
    }
}
*/