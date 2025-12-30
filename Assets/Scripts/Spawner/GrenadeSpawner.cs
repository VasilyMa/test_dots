using Unity.Entities;
using UnityEngine;

public class GrenadeSpawner : MonoBehaviour
{
    public GameObject GrenadePrefab;
    public float Firetick = 1f;

    class Baker : Baker<GrenadeSpawner>
    {
        public override void Bake(GrenadeSpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new LauncherComponent
            {
                GrenadePrefab = GetEntity(authoring.GrenadePrefab, TransformUsageFlags.Dynamic),
                Firetick = authoring.Firetick,
                GrenadesPerShot = 1,
                Delay = 0f
            });
        }
    }
}
