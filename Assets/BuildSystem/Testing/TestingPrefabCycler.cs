using UnityEngine;
using UnityEngine.InputSystem;

public class TestingPrefabCycler : MonoBehaviour
{
    public TestingBuildManager buildManager;
    public BuildingData[] buildings;

    public InputAction switchAction;

    private int currentIndex = 0;

    private void Start()
    {
        buildManager.SetBuildingData(buildings[currentIndex]);
        currentIndex = (currentIndex + 1) % buildings.Length;
    }

    private void OnEnable()
    {
        switchAction.Enable();
    }

    private void OnDisable()
    {
        switchAction.Disable();
    }

    private void Update()
    {
        if (switchAction.WasPressedThisFrame())
        {
            buildManager.SetBuildingData(buildings[currentIndex]);
            currentIndex = (currentIndex + 1) % buildings.Length;
        }
    }
}
