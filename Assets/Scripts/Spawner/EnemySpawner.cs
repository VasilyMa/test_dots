using UnityEngine;
using Unity.Entities;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public float SpawnInterval = 2f;

    class Baker : Baker<EnemySpawner>
    {
        public override void Bake(EnemySpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemySpawnerComponent
            {
                EnemyPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.SpawnInterval,
                Timer = 0f
            });
        }
    }
}