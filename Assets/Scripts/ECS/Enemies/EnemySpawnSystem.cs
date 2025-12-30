using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var em = state.EntityManager;

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
        {
            // Получаем текущее количество врагов
            var enemyQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemyComponent>()
                .Build();

            int aliveEnemies = enemyQuery.CalculateEntityCount();

            // Проверяем лимит
            if (aliveEnemies >= spawner.ValueRW.MaxEnemiesAlive)
                continue;

            // Таймер спавна
            spawner.ValueRW.Timer += deltaTime;
            if (spawner.ValueRW.Timer < spawner.ValueRW.SpawnInterval)
                continue;

            spawner.ValueRW.Timer = 0f;

            // Спавним врага
            Entity enemy = em.Instantiate(spawner.ValueRO.EnemyPrefab);

            // Задаем случайную позицию вокруг спавнера
            float3 randomPos = new float3(
                UnityEngine.Random.Range(-5f, 5f),
                0f,
                UnityEngine.Random.Range(-5f, 5f)
            );

            em.SetComponentData(enemy, LocalTransform.FromPosition(randomPos));
        }
    }
}
