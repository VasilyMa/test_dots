using Unity.Entities;
using UnityEngine;

public class GrenadeSpawner : MonoBehaviour
{
    public GameObject GrenadePrefab;
    public float Firetick = 2f;

    class Baker : Baker<GrenadeSpawner>
    {
        public override void Bake(GrenadeSpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new LauncherComponent
            {
                GrenadePrefab = GetEntity(authoring.GrenadePrefab, TransformUsageFlags.Dynamic),
                Firetick = authoring.Firetick,
                Delay = 0f
            });
        }
    }
}
