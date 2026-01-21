using UnityEngine;

public class Level : MonoBehaviour
{
    public BuildGrid BuildGrid => buildGrid;
    public BuildingSpawner BuildingSpawner => buildingSpawner;
    public Transform SpawnPointsParent => spawnPointsParent;

    [SerializeField] protected bool isLobby = false;

    [SerializeField] private BuildGrid buildGrid;
    [SerializeField] private BuildingSpawner buildingSpawner;
    [SerializeField] private Transform spawnPointsParent;

    protected void Start()
    {
        EventBus.Instance?.OnLevelLoaded?.Invoke(this, isLobby);
    }
}
