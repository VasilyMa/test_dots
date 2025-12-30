using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class ExplodeEntity : MonoBehaviour
{
    public float Duration = 0.5f;
    public float MaxRadius = 5f;

    public class Baker : Baker<ExplodeEntity>
    {
        public override void Bake(ExplodeEntity authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new ExplodeComponent
            {
                MaxRadius = authoring.MaxRadius,
                Duration = authoring.Duration,
                Timer = 0f // всегда начинаем с нуля
            });

            // Если хотим сразу задать цвет через URP
            var renderer = authoring.GetComponent<Renderer>();
            if (renderer != null)
            {
                var col = renderer.sharedMaterial.color;
                AddComponent(entity, new URPMaterialPropertyBaseColor
                {
                    Value = new float4(col.r, col.g, col.b, col.a)
                });
            }
        }
    }
}
