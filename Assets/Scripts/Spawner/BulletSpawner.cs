using UnityEngine;
using Unity.Entities;

public class BulletSpawner : MonoBehaviour
{ 
    public GameObject BulletPrefab;
    public float Firetick = 0.2f;

    class Baker : Baker<BulletSpawner>
    {
        public override void Bake(BulletSpawner authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new WeaponComponent
            {
                BulletPrefab = GetEntity(authoring.BulletPrefab, TransformUsageFlags.Dynamic),
                Firetick = authoring.Firetick,
                BulletsPerShot = 1,
                Delay = 0f
            }); 
        }
    }
}
