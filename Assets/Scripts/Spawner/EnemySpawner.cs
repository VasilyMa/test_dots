using UnityEngine;
using Unity.Entities;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval = 1f;
    public int MaxSpawnCount = 10;
    public int EnemyCountPerSpawn = 1;

    class Baker : Baker<EnemySpawner>
    {
        public override void Bake(EnemySpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemySpawnerComponent
            {
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.SpawnInterval,
                MaxEnemiesAlive = authoring.MaxSpawnCount,
                EnemyPerSpawn = authoring.EnemyCountPerSpawn,
                Timer = 0f
            });
        }
    }
}