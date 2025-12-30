using UnityEngine;
using Unity.Entities;

public class PlayerEntity : MonoBehaviour
{
    public float Attack = 1f;
    public float Health = 10f;
    public float MoveSpeed = 5f;

    class Baker : Baker<PlayerEntity>
    {
        public override void Bake(PlayerEntity authoring)
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

            AddComponent<InputComponent>(entity);
            AddComponent<PlayerComponent>(entity);
        }
    }
}