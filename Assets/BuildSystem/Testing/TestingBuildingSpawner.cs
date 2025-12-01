using UnityEngine;

public class TestingBuildingSpawner : MonoBehaviour
{
    public BuildingSpawner buildingSpawner;
    public BuildingData[] buildings;
    public int buildingCount = 5;

    private void Start()
    {
        this.CallNextFrame(CallBuildingSpawner);
    }

    private void CallBuildingSpawner()
    {
        buildingSpawner.SpawnBuildings(buildings, buildingCount);
    }
}
