using UnityEngine;
using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity EnemyPrefab;
    public float SpawnInterval;
    public float Timer;
    public int MaxEnemiesAlive;
    public int EnemyPerSpawn;
}
