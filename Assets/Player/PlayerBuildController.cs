using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildController : MonoBehaviour
{
    public BuildGrid grid;
    public BuildingData currentBuildingData;

    public Action OnBuildingPlaced;

    [SerializeField] private InputAction cursorPositionAction;
    [SerializeField] private InputAction placeAction;
    [SerializeField] private float cursorSpeed = 400f;
    [SerializeField] private float screenMargin = 20f;

    [SerializeField] private Color validColor = Color.white;
    [SerializeField] private Color invalidColor = Color.red;

    private Camera mainCamera;
    private Vector2 screenPos;

    private BuildingGhost buildingGhost;

    private void Awake()
    {
        buildingGhost = transform.Find("BuildingGhost").GetComponent<BuildingGhost>();
    }

    private void Start()
    {
        mainCamera = Camera.main;

        buildingGhost.transform.localScale = new Vector3(grid.cellSize, grid.cellSize, 1);

        if (currentBuildingData != null)
            buildingGhost.ShowBuilding(currentBuildingData);
    }

    private void OnEnable()
    {
        cursorPositionAction.Enable();
        placeAction.Enable();

        screenPos = new Vector2(Screen.width * .5f, Screen.height * .5f);
    }

    private void OnDisable()
    {
        cursorPositionAction.Disable();
        placeAction.Disable();
    }

    public void Init(BuildGrid buildGrid, BuildingData buildingData)
    {
        grid = buildGrid;
        currentBuildingData = buildingData;

        buildingGhost.ShowBuilding(buildingData);
    }

    private void Update()
    {
        if (grid == null || currentBuildingData == null)
            return;

        Vector2 moveInput = cursorPositionAction.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            screenPos += cursorSpeed * Time.deltaTime * moveInput;

            screenPos.x = Mathf.Clamp(screenPos.x, screenMargin, Screen.width - screenMargin);
            screenPos.y = Mathf.Clamp(screenPos.y, screenMargin, Screen.height - screenMargin);
        }

        var worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -mainCamera.transform.position.z));

        var cellPos = grid.IsPositionInsideGrid(worldPos) ? grid.GetCellPosition(worldPos) : worldPos;
        var canBuild = grid.CanPlaceBuilding(cellPos, currentBuildingData);

        buildingGhost.transform.position = cellPos;
        buildingGhost.SetTint(canBuild ? validColor : invalidColor);

        if (canBuild && placeAction.WasPressedThisFrame())
        {
            grid.AddBuilding(cellPos, currentBuildingData);
            OnBuildingPlaced?.Invoke();
        }
    }
}
