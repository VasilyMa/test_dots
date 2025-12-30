using Unity.Entities;
using Unity.Mathematics;

public struct BulletComponent : IComponentData
{
    public Entity Target;
    public float3 Direction;
    public float Speed;
    public float Damage;
    public float Lifetime;
}
