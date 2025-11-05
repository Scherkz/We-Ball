using UnityEngine;
using UnityEngine.InputSystem;

public class TestingPrefabCycler : MonoBehaviour
{
    public BuildManager buildManager;
    public BuildingData[] buildings;

    private int currentIndex = 0;

    private void Start()
    {
        buildManager.SetBuildingData(buildings[currentIndex]);
        currentIndex = (currentIndex + 1) % buildings.Length;
    }

    private void Update()
    {
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            buildManager.SetBuildingData(buildings[currentIndex]);
            currentIndex = (currentIndex + 1) % buildings.Length;
        }
    }
}
