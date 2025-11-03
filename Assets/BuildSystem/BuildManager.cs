using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public BuildGrid grid;

    public Color validColor = Color.white;
    public Color invalidColor = Color.red;

    [SerializeField] private Camera mainCamera;

    private Building building;
    [SerializeField] private BuildingData currentBuildingData;

    private void Awake()
    {
        building = transform.Find("DisplayBuilding").GetComponent<Building>();
    }

    private void Start()
    {
        building.transform.localScale = new Vector3(grid.cellSize, grid.cellSize, 1);

        if (currentBuildingData != null)
            building.Init(currentBuildingData);
    }

    public void SetBuildingData(BuildingData buildingData)
    {
        currentBuildingData = buildingData;
        building.Init(buildingData);
    }

    private void Update()
    {
        if (currentBuildingData == null)
            return;

        var mousePos = Mouse.current.position.ReadValue();
        var mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -mainCamera.transform.position.z));

        var cellPos = grid.IsPositionInsideGrid(mouseWorldPos) ? grid.GetCellPosition(mouseWorldPos) : mouseWorldPos;

        building.transform.localPosition = cellPos;
        building.SetTint(grid.CanPlaceBuilding(cellPos, currentBuildingData) ? validColor : invalidColor);
    }
}
