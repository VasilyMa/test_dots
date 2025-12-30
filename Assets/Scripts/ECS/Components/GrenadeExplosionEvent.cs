using Unity.Entities;
using Unity.Mathematics;

public struct GrenadeExplosionEvent : IComponentData
{
    public float3 Position;       // Где взрыв
    public float Radius;          // Радиус взрыва
    public float Damage;          // Урон
}
