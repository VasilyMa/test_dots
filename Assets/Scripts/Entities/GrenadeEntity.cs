using Unity.Entities;
using UnityEngine;

public class GrenadeEntity : MonoBehaviour
{  
    public float Speed;            // Скорость продвижения (ед/сек)
    public float ArcHeight;        // Максимальная высота дуги
    public float Damage;           // Урон
    public float ExplosionRadius;  // Радиус AoE

    public class Baker : Baker<GrenadeEntity>
    {
        public override void Bake(GrenadeEntity authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GrenadeComponent
            { 
                Damage = authoring.Damage,
                Speed = authoring.Speed,
                ArcHeight = authoring.ArcHeight,
                ExplosionRadius = authoring.ExplosionRadius 
            });
        }
    }
}