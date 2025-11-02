using System.Collections.Generic;
using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    struct GridData
    {
        public BuildingData buildingData;
        public GameObject instance;

        public readonly bool IsOccupied => buildingData != null;
    }

    public float cellSize = 0.25f;
    public Vector2Int cellCount;

    [SerializeField] private GameObject buildingPrefab;

    private GridData[] grid;

    private void Awake()
    {
        grid = new GridData[cellCount.x * cellCount.y];
    }

    public bool CanPlaceBuilding(Vector3 position, BuildingData buildingData)
    {
        var cellCoords = GetCellCoords(position);
        var cellIndex = GetCellIndex(cellCoords.x, cellCoords.y);
        foreach (var cellOffset in IterateBuildingCells(buildingData))
        {
            if (cellCoords.x + cellOffset.x >= cellCount.x)
                return false;

            if (cellCoords.y + cellOffset.y >= cellCount.y)
                return false;

            if (grid[cellIndex + GetCellIndex(cellOffset.x, cellOffset.y)].IsOccupied)
                return false;
        }

        return true;
    }

    public bool AddBuilding(Vector3 position, BuildingData buildingData)
    {
        if (!CanPlaceBuilding(position, buildingData))
            return false;

        // instanciate building at position
        var instance = Instantiate(buildingPrefab);

        instance.transform.SetParent(transform);
        instance.transform.localPosition = GetCellPosition(position);

        var building = instance.GetComponent<Building>();
        building.Init(buildingData);

        var GridData = new GridData
        {
            buildingData = buildingData,
            instance = instance,
        };

        // store building reference in grid
        var cellIndex = GetCellIndex(position);
        foreach (var cellOffset in IterateBuildingCells(buildingData))
        {
            grid[cellIndex + GetCellIndex(cellOffset.x, cellOffset.y)] = GridData;
        }

        return true;
    }

    private IEnumerable<Vector2Int> IterateBuildingCells(BuildingData buildingData)
    {
        for (int x = 0; x < buildingData.cellCount.x; x++)
        {
            for (int y = 0; y < buildingData.cellCount.y; y++)
            {
                yield return new Vector2Int(x, y);
            }
        }
    }

    private int GetCellIndex(Vector3 position)
    {
        var cellCoords = GetCellCoords(position);
        return GetCellIndex(cellCoords.x, cellCoords.y);
    }

    private int GetCellIndex(int x, int y)
    {
        return x + y * cellCount.x;
    }

    private Vector3 GetCellPosition(Vector3 position)
    {
        var cellCoords = GetCellCoords(position);
        return new Vector3(cellCoords.x * cellSize, cellCoords.y * cellSize, position.z);
    }

    private Vector2Int GetCellCoords(Vector3 position)
    {
        int x = (int) Mathf.Floor(position.x / cellSize);
        int y = (int) Mathf.Floor(position.y / cellSize);
        return new Vector2Int(x, y);
    }
}
