using Unity.Entities;
using UnityEngine;

public struct LauncherComponent : IComponentData
{
    public Entity GrenadePrefab;
    public float Firetick;
    public float Delay;
    public int GrenadesPerShot;
}
