using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Collections;

public class DebugPanel : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField SpawnRateInput;
    public TMP_InputField MaxEnemiesInput;
    public TMP_InputField BulletsPerFireInput;
    public TMP_InputField GrenadesPerFireInput;

    [Header("Sliders")]
    public Slider BulletFireRateSlider;
    public Slider GrenadeFireRateSlider;
    public TMP_Text BulletFireRateText;
    public TMP_Text GrenadeFireRateText;

    void Start()
    {
        // Input Fields
        SpawnRateInput.onValueChanged.AddListener(OnSpawnRateChanged);
        MaxEnemiesInput.onValueChanged.AddListener(OnMaxEnemiesChanged);
        BulletsPerFireInput.onValueChanged.AddListener(OnBulletsPerFireChanged);
        GrenadesPerFireInput.onValueChanged.AddListener(OnGrenadesPerFireChanged);

        // Sliders
        BulletFireRateSlider.onValueChanged.AddListener(OnBulletFireRateChanged);
        GrenadeFireRateSlider.onValueChanged.AddListener(OnGrenadeFireRateChanged);
    }

    // ---------------- EnemySpawnerComponent ----------------
    private void OnSpawnRateChanged(string value)
    {
        if (!float.TryParse(value, out float spawnRate)) return;

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(EnemySpawnerComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<EnemySpawnerComponent>(entity);
                data.SpawnInterval = spawnRate;
                em.SetComponentData(entity, data);
            }
        }
    }

    private void OnMaxEnemiesChanged(string value)
    {
        if (!int.TryParse(value, out int maxEnemies)) return;

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(EnemySpawnerComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<EnemySpawnerComponent>(entity);
                data.MaxEnemiesAlive = maxEnemies;
                em.SetComponentData(entity, data);
            }
        }
    }

    // ---------------- WeaponComponent ----------------
    private void OnBulletFireRateChanged(float value)
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(WeaponComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<WeaponComponent>(entity);
                data.Firetick = value;
                em.SetComponentData(entity, data);
            }
        }

        BulletFireRateText.text = value.ToString("F1");
    }

    private void OnBulletsPerFireChanged(string value)
    {
        if (!int.TryParse(value, out int count)) return;

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(WeaponComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<WeaponComponent>(entity);
                data.BulletsPerShot = count;
                em.SetComponentData(entity, data);
            }
        }
    }

    // ---------------- GrenadeComponent ----------------
    private void OnGrenadeFireRateChanged(float value)
    {
        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(LauncherComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<LauncherComponent>(entity);
                data.Firetick = value;
                em.SetComponentData(entity, data);
            }
        }

        GrenadeFireRateText.text = value.ToString("F1");
    }

    private void OnGrenadesPerFireChanged(string value)
    {
        if (!int.TryParse(value, out int count)) return;

        var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = em.CreateEntityQuery(typeof(LauncherComponent));

        using (var entities = query.ToEntityArray(Allocator.Temp))
        {
            foreach (var entity in entities)
            {
                var data = em.GetComponentData<LauncherComponent>(entity);
                data.GrenadesPerShot = count;
                em.SetComponentData(entity, data);
            }
        }
    }
}
