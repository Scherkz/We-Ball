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
    private Vector3 offset;
    private SpriteRenderer gridVisualisation;

    private void Awake()
    {
        grid = new GridData[cellCount.x * cellCount.y];
        offset = new Vector3(-cellCount.x * cellSize * 0.5f, -cellCount.y * cellSize * 0.5f, 0);

        gridVisualisation = transform.Find("GridVisualisation").GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        gridVisualisation.size = cellCount;
        gridVisualisation.transform.localPosition = offset;
        gridVisualisation.transform.localScale = new Vector3(cellSize, -cellSize, 1);
    }

    public bool CanPlaceBuilding(Vector3 position, BuildingData buildingData)
    {
        var localPosition = transform.InverseTransformPoint(position);
        if (!_IsPositionInsideGrid(localPosition))
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

    public bool IsPositionInsideGrid(Vector3 position)
    {
        var localPosition = transform.InverseTransformPoint(position);
        return _IsPositionInsideGrid(localPosition);
    }

    public Vector3 GetCellPosition(Vector3 position)
    {
        var localPosition = transform.InverseTransformPoint(position);
        var cellCoords = GetCellCoords(localPosition);
        var localCellPosition = new Vector3(offset.x + cellCoords.x * cellSize, offset.y + cellCoords.y * cellSize, localPosition.z);
        return transform.TransformPoint(localCellPosition);
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

    private bool _IsPositionInsideGrid(Vector3 localPosition)
    {
        if (localPosition.x < offset.x)
            return false;

        if (localPosition.y < offset.y)
            return false;

        var farCorner = offset + new Vector3(cellCount.x * cellSize, cellCount.y * cellSize, 0);
        if (localPosition.x >= farCorner.x)
            return false;

        if (localPosition.y >= farCorner.y)
            return false;

        return true;
    }

    private Vector2Int GetCellCoords(Vector3 localPosition)
    {
        int x = (int) Mathf.Floor((localPosition.x - offset.x) / cellSize);
        int y = (int) Mathf.Floor((localPosition.y - offset.y) / cellSize);
        return new Vector2Int(x, y);
    }
}
