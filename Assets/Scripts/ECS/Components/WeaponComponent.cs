using Unity.Entities;

public struct WeaponComponent : IComponentData
{ 
    public Entity BulletPrefab;
    public float Firetick;
    public float Delay;
    public int BulletsPerShot;
}
