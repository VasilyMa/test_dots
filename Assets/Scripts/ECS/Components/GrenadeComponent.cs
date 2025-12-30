using Unity.Entities;
using Unity.Mathematics;

public struct GrenadeComponent : IComponentData
{
    public float3 StartPosition;   // Откуда бросаем
    public float3 TargetPosition;  // Куда летим
    public float Progress;         // 0 → 1
    public float Speed;            // Скорость продвижения (ед/сек)
    public float ArcHeight;        // Максимальная высота дуги
    public float Damage;           // Урон
    public float ExplosionRadius;  // Радиус AoE
}
