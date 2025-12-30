using UnityEngine;
using Unity.Entities;

public class BulletEntity : MonoBehaviour
{
    public float Lifetime = 5f;
    public float Speed = 10f;
    public float Damage = 2f;

    public class Baker : Baker<BulletEntity>
    {
        public override void Bake(BulletEntity authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BulletComponent
            {
                Lifetime = authoring.Lifetime,
                Damage = authoring.Damage,
                Speed = authoring.Speed,
                Target = Entity.Null
            });
        }
    }
}
