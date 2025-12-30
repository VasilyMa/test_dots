using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections;

public class InputBridge : MonoBehaviour
{
    public FloatingJoystick Joystick;

    private EntityManager _entityManager;
    private Entity _playerEntity;
     
    IEnumerator Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager; 

        while (true)
        {
            var query = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerComponent>(),
                ComponentType.ReadWrite<InputComponent>()
            );

            if (query.CalculateEntityCount() > 0)
            {
                _playerEntity = query.GetSingletonEntity();
                break; // игрок найден, выходим из цикла
            }

            yield return null; // ждём следующий кадр
        }
    }

    void Update()
    { 
        if (_playerEntity == Entity.Null || !_entityManager.Exists(_playerEntity))
            return;

        // Читаем движение с джойстика
        float2 move = new float2(Joystick.Horizontal, Joystick.Vertical);

        // Обновляем ECS компонент игрока
        var input = _entityManager.GetComponentData<InputComponent>(_playerEntity);
        input.Move = move;
        _entityManager.SetComponentData(_playerEntity, input);
    }
}
