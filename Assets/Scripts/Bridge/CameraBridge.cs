using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Collections;

public class CameraBridge : MonoBehaviour
{
    public float Height = 2f;
    public float Distance = 5f;
    public float FollowSpeed = 5f;

    private EntityManager _em;
    private Entity _playerEntity;

    IEnumerator Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        // ⏳ Ждём пока появится игрок
        while (true)
        {
            if (_em.World.IsCreated)
            {
                var query = _em.CreateEntityQuery(
                    ComponentType.ReadOnly<PlayerComponent>(),
                    ComponentType.ReadOnly<LocalTransform>()
                );

                if (!query.IsEmptyIgnoreFilter)
                {
                    _playerEntity = query.GetSingletonEntity();
                    break;
                }
            }

            yield return null; // ждём следующий кадр
        }
    }

    void LateUpdate()
    {
        if (_playerEntity == Entity.Null || !_em.Exists(_playerEntity))
            return;

        var playerTransform = _em.GetComponentData<LocalTransform>(_playerEntity);
        Vector3 playerPos = (Vector3)playerTransform.Position;

        Vector3 targetPos = playerPos + new Vector3(0, Height, -Distance);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            FollowSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.LookRotation(playerPos - transform.position);
    }
}
