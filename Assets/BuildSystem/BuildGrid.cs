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

    private GridData[] grid;
    private SpriteRenderer gridVisualisation;

    private void OnValidate()
    {
        var gridVis = transform.Find("GridVisualisation").GetComponent<SpriteRenderer>();
        gridVis.size = cellCount;
        gridVis.transform.localScale = new Vector3(cellSize, -cellSize, 1);
    }

    private void Awake()
    {
        grid = new GridData[cellCount.x * cellCount.y];

        gridVisualisation = transform.Find("GridVisualisation").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gridVisualisation.size = cellCount;
        gridVisualisation.transform.localScale = new Vector3(cellSize, -cellSize, 1);
    }

    public void ShowGrid(bool enabled)
    {
        gridVisualisation.enabled = enabled;
    }

    public bool AddBuilding(Vector3 position, BuildingData buildingData)
    {
        if (!CanPlaceBuilding(position, buildingData))
            return false;

        var localPosition = transform.InverseTransformPoint(position);

        // instanciate building at position
        var instance = Instantiate(buildingData.prefab);

        instance.transform.SetParent(transform);
        instance.transform.localPosition = InternalGetCellPosition(localPosition);
        instance.transform.localScale = new Vector3(cellSize, cellSize, 1);

        var GridData = new GridData
        {
            buildingData = buildingData,
            instance = instance,
        };

        // store building reference in grid
        var cellIndex = GetCellIndex(localPosition);
        foreach (var cellOffset in IterateBuildingCells(buildingData))
        {
            grid[cellIndex + GetCellIndex(cellOffset.x, cellOffset.y)] = GridData;
        }

        return true;
    }

    public bool CanPlaceBuilding(Vector3 position, BuildingData buildingData)
    {
        var localPosition = transform.InverseTransformPoint(position);
        if (!InternalIsPositionInsideGrid(localPosition))
            return false;

        var cellCoords = GetCellCoords(localPosition);
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

    public bool IsPositionInsideGrid(Vector3 position)
    {
        var localPosition = transform.InverseTransformPoint(position);
        return InternalIsPositionInsideGrid(localPosition);
    }

    public Vector3 GetCellPosition(Vector3 position)
    {
        var localPosition = transform.InverseTransformPoint(position);
        var localCellPosition = InternalGetCellPosition(localPosition);
        return transform.TransformPoint(localCellPosition);
    }

    private Vector3 InternalGetCellPosition(Vector3 localPosition)
    {
        var cellCoords = GetCellCoords(localPosition);
        return new Vector3(cellCoords.x * cellSize, cellCoords.y * cellSize, localPosition.z);
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

    private int GetCellIndex(Vector3 localPosition)
    {
        var cellCoords = GetCellCoords(localPosition);
        return GetCellIndex(cellCoords.x, cellCoords.y);
    }

    private int GetCellIndex(int x, int y)
    {
        return x + y * cellCount.x;
    }

    private bool InternalIsPositionInsideGrid(Vector3 localPosition)
    {
        if (localPosition.x < 0)
            return false;

        if (localPosition.y < 0)
            return false;

        var farCorner = new Vector3(cellCount.x * cellSize, cellCount.y * cellSize, 0);
        if (localPosition.x >= farCorner.x)
            return false;

        if (localPosition.y >= farCorner.y)
            return false;

        return true;
    }

    private Vector2Int GetCellCoords(Vector3 localPosition)
    {
        int x = (int) Mathf.Floor(localPosition.x / cellSize);
        int y = (int) Mathf.Floor(localPosition.y / cellSize);
        return new Vector2Int(x, y);
    }
}
