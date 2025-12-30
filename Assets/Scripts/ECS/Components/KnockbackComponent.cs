using Unity.Entities;
using Unity.Mathematics; 

public struct KnockbackComponent : IComponentData
{
    public float3 Direction;
    public float Force;
    public float Duration;
    public float Timer;
}
