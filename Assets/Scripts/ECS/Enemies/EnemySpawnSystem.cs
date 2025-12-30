using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        _random = new Random(12345);
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var em = state.EntityManager;

        // Получаем игрока
        if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out var playerEntity))
            return;

        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        // Считаем текущее количество врагов
        var enemyQuery = SystemAPI.QueryBuilder()
            .WithAll<EnemyComponent>()
            .Build();
        int aliveEnemies = enemyQuery.CalculateEntityCount();

        foreach (var spawner in SystemAPI.Query<RefRW<EnemySpawnerComponent>>())
        {
            // Таймер спавна
            spawner.ValueRW.Timer += deltaTime;
            if (spawner.ValueRW.Timer < spawner.ValueRW.SpawnInterval)
                continue;

            // Проверка лимита врагов
            if (aliveEnemies >= spawner.ValueRW.MaxEnemiesAlive)
                continue;

            spawner.ValueRW.Timer = 0f;

            // Спавн врага
            Entity enemy = em.Instantiate(spawner.ValueRO.EnemyPrefab);

            // Случайное смещение вокруг игрока
            float3 offset = new float3(
                _random.NextFloat(-5f, 5f),
                0f,
                _random.NextFloat(-5f, 5f)
            );

            em.SetComponentData(enemy, LocalTransform.FromPosition(playerTransform.Position + offset));

            aliveEnemies++;
        }
    }
}
