using Unity.Entities;

public struct ExplodeComponent : IComponentData
{ 
    public float Timer;           // Время жизни
    public float Duration;        // Общая длительность анимации
    public float MaxRadius;       // Максимальный радиус визуальной сферы
}
