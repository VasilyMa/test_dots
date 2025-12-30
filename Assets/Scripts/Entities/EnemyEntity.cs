using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    public float Attack;
    public float Health;
    public float MoveSpeed;

    class Baker : Baker<EnemyEntity>
    {
        public override void Bake(EnemyEntity authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new MoveComponent
            {
                Speed = authoring.MoveSpeed
            }); 
            AddComponent(entity, new AttackComponent
            {
                Damage = authoring.Attack
            }); 
            AddComponent(entity, new HealthComponent
            {
                CurrentHealth = authoring.Health,
                MaxHealth = authoring.Health
            });

            var renderer = authoring.GetComponent<Renderer>();
            if (renderer != null)
            {
                var col = renderer.sharedMaterial.color;
                var colF4 = new float4(col.r, col.g, col.b, col.a);

                // Компонент для flash
                AddComponent(entity, new URPMaterialPropertyBaseColor { Value = colF4 });

                // Сохраняем исходный цвет
                AddComponent(entity, new BaseColorComponent { Value = colF4 });
            }

            AddComponent<EnemyComponent>(entity);
        }
    }
}
