using UnityEngine;
using Unity.Entities;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject PlayerPrefab; // GameObject prefab ñ PlayerAuthoring

    private Entity _playerEntity;
    private Entity _playerPrefabEntity;

    void Awake()
    { 
    }

    public Entity GetPlayerEntity() => _playerEntity;
}
