using Unity.Entities;
using UnityEngine;

public class ExplodeSpawner : MonoBehaviour
{
    public GameObject ExplodePrefab; 

    class Baker : Baker<ExplodeSpawner>
    {
        public override void Bake(ExplodeSpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ExploderComponent
            {
                Explode = GetEntity(authoring.ExplodePrefab, TransformUsageFlags.Dynamic), 
            });
        }
    }
}