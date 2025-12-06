using UnityEngine;

public class Level : MonoBehaviour
{
    public BuildGrid BuildGrid => buildGrid;
    public BuildingSpawner BuildingSpawner => buildingSpawner;
    public Transform SpawnPointsParent => spawnPointsParent;

    [SerializeField] private BuildGrid buildGrid;
    [SerializeField] private BuildingSpawner buildingSpawner;
    [SerializeField] private Transform spawnPointsParent;

    private void Start()
    {
        EventBus.Instance?.OnLevelLoaded?.Invoke(this);
    }
}
