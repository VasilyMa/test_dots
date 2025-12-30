using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class InputBridge : MonoBehaviour
{
    public FloatingJoystick Joystick;

    private EntityManager _entityManager;
    private Entity _playerEntity;

    void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void Update()
    {
        // Если игрок ещё не найден или был удалён, ищем его
        if (_playerEntity == Entity.Null || !_entityManager.Exists(_playerEntity))
        {
            var query = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerComponent>(),
                ComponentType.ReadWrite<InputComponent>()
            );

            if (query.CalculateEntityCount() == 0)
                return; // игрок ещё не создан

            _playerEntity = query.GetSingletonEntity();
        }

        // Получаем движение с джойстика
        float2 move = new float2(
            Joystick.Horizontal,
            Joystick.Vertical
        );

        // Обновляем ECS компонент
        var input = _entityManager.GetComponentData<InputComponent>(_playerEntity);
        input.Move = move;
        _entityManager.SetComponentData(_playerEntity, input);
    }
}
