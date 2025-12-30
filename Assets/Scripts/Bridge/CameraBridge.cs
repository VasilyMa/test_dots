using UnityEngine;
using Unity.Entities;
using Unity.Transforms; 

public class CameraBridge : MonoBehaviour
{
    [Header("Настройки камеры")]
    public float Height = 2f;
    public float Distance = 5f;
    public float FollowSpeed = 5f;

    private EntityManager _entityManager;
    private Entity _playerEntity;

    void Awake()
    {
        // Получаем EntityManager текущего мира
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    void LateUpdate()
    {
        // Если игрок ещё не найден, пробуем найти его
        if (_playerEntity == Entity.Null || !_entityManager.Exists(_playerEntity))
        {
            var query = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>());
            if (query.CalculateEntityCount() == 0)
                return; // игрок ещё не создан

            _playerEntity = query.GetSingletonEntity();
        }

        // Получаем позицию игрока из ECS
        var playerTransform = _entityManager.GetComponentData<LocalTransform>(_playerEntity);
        Vector3 playerPos = (Vector3)playerTransform.Position;

        // Вычисляем целевую позицию камеры
        Vector3 targetPos = playerPos + new Vector3(0, Height, -Distance);

        // Плавное движение камеры
        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);

        // Смотрим на игрока
        transform.rotation = Quaternion.LookRotation(playerPos - transform.position);
    }
}
