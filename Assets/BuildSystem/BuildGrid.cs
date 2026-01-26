using System.Collections.Generic;
using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    struct CellData
    {
        public BuildingData buildingData;
        public GameObject instance;

        public readonly bool IsOccupied => buildingData != null;
    }

    public float cellSize = 0.25f;
    public Vector2Int cellCount;

    private CellData[] grid;
    private SpriteRenderer gridVisualisation;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool debug;

    private void Update()
    {
        if (!debug)
            return;

        for (int x = 0; x < cellCount.x; x++)
        {
            for (int y = 0; y < cellCount.y; y++)
            {
                int index = GetCellIndex(x, y);
                var cell = grid[index];

                if (cell.IsOccupied)
                {
                    var cellBLPosition = new Vector3(x, y, 0);
                    var cellTRPosition = new Vector3(x + 1, y + 1, 0);
                    var globalBLPosition = transform.TransformPoint(cellBLPosition * cellSize);
                    var globalTRPosition = transform.TransformPoint(cellTRPosition * cellSize);
                    Debug.DrawLine(globalBLPosition, globalTRPosition, Color.red);
                }
            }
        }
    }
#endif

    private void Awake()
    {
        grid = new CellData[cellCount.x * cellCount.y];

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

    public bool AddBuilding(Vector3 position, BuildingData buildingData, Building.Rotation rotation)
    {
        if (!CanPlaceBuilding(position, buildingData, rotation))
            return false;

        var localPosition = transform.InverseTransformPoint(position);

        // instanciate building at position
        var instance = Instantiate(buildingData.prefab);

        instance.transform.SetParent(transform);
        instance.transform.localPosition = InternalGetCellPosition(localPosition);
        instance.transform.localScale = new Vector3(cellSize, cellSize, 1);

        var building = instance.GetComponent<Building>();
        building.SetRotation(buildingData, rotation);
        building.CallNextFrame(building.Init);

        var cellIndex = GetCellIndex(localPosition);

        // in case of an anti building clear all the cell instead of storing the building
        if (buildingData.isAntiBuilding)
        {
            foreach (var cellOffset in IterateBuildingCells(buildingData, rotation))
            {
                RemoveBuildingFromCell(cellIndex + GetCellIndex(cellOffset.x, cellOffset.y));
            }
            return true;
        }

        // store building reference in grid
        var CellData = new CellData
        {
            buildingData = buildingData,
            instance = instance,
        };

        foreach (var cellOffset in IterateBuildingCells(buildingData, rotation))
        {
            grid[cellIndex + GetCellIndex(cellOffset.x, cellOffset.y)] = CellData;
        }

        return true;
    }

    public bool CanPlaceBuilding(Vector3 position, BuildingData buildingData, Building.Rotation rotation)
    {
        var localPosition = transform.InverseTransformPoint(position);
        if (!InternalIsPositionInsideGrid(localPosition))
            return false;

        var cellCoords = GetCellCoords(localPosition);
        var cellIndex = GetCellIndex(cellCoords.x, cellCoords.y);

        bool isOutsideGrid(Vector2Int cellOffset) =>
            cellCoords.x + cellOffset.x >= cellCount.x ||
            cellCoords.y + cellOffset.y >= cellCount.y;
        bool isCellOccupied(Vector2Int cellOffset) =>
            !buildingData.isAntiBuilding &&
            grid[cellIndex + GetCellIndex(cellOffset.x, cellOffset.y)].IsOccupied;

#if UNITY_EDITOR
        if (debug)
        {
            foreach (var cellOffset in IterateBuildingCells(buildingData, rotation))
            {
                bool invalid = isOutsideGrid(cellOffset) || isCellOccupied(cellOffset);

                var cellTLPosition = new Vector3(cellCoords.x + cellOffset.x, cellCoords.y + cellOffset.y + 1, 0);
                var cellBRPosition = new Vector3(cellCoords.x + cellOffset.x + 1, cellCoords.y + cellOffset.y, 0);
                var globalTLPosition = transform.TransformPoint(cellTLPosition * cellSize);
                var globalBRPosition = transform.TransformPoint(cellBRPosition * cellSize);
                Debug.DrawLine(globalTLPosition, globalBRPosition, invalid ? Color.deepPink : Color.green);
            }
        }
#endif

        foreach (var cellOffset in IterateBuildingCells(buildingData, rotation))
        {
            if (isOutsideGrid(cellOffset) || isCellOccupied(cellOffset))
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

    public void RemoveBuildingAtPosition(Vector3 position)
    {
        var localPosition = transform.InverseTransformPoint(position);

        var cellIndex = GetCellIndex(localPosition);
        RemoveBuildingFromCell(cellIndex);
    }

    public float GetOccupationPercentage()
    {
        int count = 0;
        foreach (var cell in grid)
        {
            if (cell.IsOccupied)
            {
                count++;
            }
        }

        return count / (float)grid.Length;
    }

    private void RemoveBuildingFromCell(int cellIndex)
    {
        // find cell of building origin
        var cellData = grid[cellIndex];
        if (!cellData.IsOccupied)
            return;

        var instancePosition = cellData.instance.transform.position;
        var localPosition = transform.InverseTransformPoint(instancePosition);

        var rotation = cellData.instance.GetComponent<Building>().rotation;

        // clear all the occupied cells
        var originCellIndex = GetCellIndex(localPosition);
        foreach (var cellOffset in IterateBuildingCells(cellData.buildingData, rotation))
        {
            var index = originCellIndex + GetCellIndex(cellOffset.x, cellOffset.y);
            if (!grid[index].IsOccupied)
                continue;

            grid[index].buildingData = null;
            grid[index].instance = null;
        }

        // destroy building instance
        Destroy(cellData.instance);
    }

    private Vector3 InternalGetCellPosition(Vector3 localPosition)
    {
        var cellCoords = GetCellCoords(localPosition);
        return new Vector3(cellCoords.x * cellSize, cellCoords.y * cellSize, localPosition.z);
    }

    private IEnumerable<Vector2Int> IterateBuildingCells(BuildingData buildingData, Building.Rotation rotation)
    {
        var size = buildingData.cellCount;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (!buildingData.UsesCell(x, y))
                    continue;

                var local = new Vector2Int(x, y);

                var rotated = RotateLocalOffset(local, size, rotation);

                yield return rotated;
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

    private Vector2Int RotateLocalOffset(Vector2Int local, Vector2Int size, Building.Rotation rotation)
    {
        int x = local.x;
        int y = local.y;
        int w = size.x;
        int h = size.y;

        switch (rotation)
        {
            default:
            case Building.Rotation.Degree0:
                return new Vector2Int(x, y);

            case Building.Rotation.Degree90:
                return new Vector2Int(y, w - 1 - x);

            case Building.Rotation.Degree180:
                return new Vector2Int(w - 1 - x, h - 1 - y);

            case Building.Rotation.Degree270:
                return new Vector2Int(h - 1 - y, x);
        }
    }
}
