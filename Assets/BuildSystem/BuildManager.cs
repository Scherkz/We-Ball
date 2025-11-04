using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public BuildGrid grid;

    public Color validColor = Color.white;
    public Color invalidColor = Color.red;

    [SerializeField] private Camera mainCamera;

    private BuildingGhost building;
    [SerializeField] private BuildingData currentBuildingData;

    private void Awake()
    {
        building = transform.Find("BuildingGhost").GetComponent<BuildingGhost>();
    }

    private void Start()
    {
        building.transform.localScale = new Vector3(grid.cellSize, grid.cellSize, 1);

        if (currentBuildingData != null)
            building.ShowBuilding(currentBuildingData);
    }

    public void SetBuildingData(BuildingData buildingData)
    {
        currentBuildingData = buildingData;
        building.ShowBuilding(buildingData);
    }

    private void Update()
    {
        if (currentBuildingData == null)
            return;

        var mousePos = Mouse.current.position.ReadValue();
        var mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -mainCamera.transform.position.z));

        var cellPos = grid.IsPositionInsideGrid(mouseWorldPos) ? grid.GetCellPosition(mouseWorldPos) : mouseWorldPos;
        var canBuild = grid.CanPlaceBuilding(cellPos, currentBuildingData);

        building.transform.position = cellPos;
        building.SetTint(canBuild ? validColor : invalidColor);

        // TODO: Replace with InputAction
        if (canBuild && Mouse.current.leftButton.IsPressed() && Mouse.current.leftButton.wasPressedThisFrame)
        {
            grid.AddBuilding(cellPos, currentBuildingData);
        }
    }
}
