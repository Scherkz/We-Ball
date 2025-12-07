using UnityEngine;
using UnityEngine.InputSystem;

public class TestingBuildManager : MonoBehaviour
{
    public BuildGrid grid;

    public Color validColor = Color.white;
    public Color invalidColor = Color.red;

    [SerializeField] private Camera mainCamera;

    private BuildingGhost building;
    [SerializeField] private BuildingData currentBuildingData;

    [SerializeField] private bool enableMouseControls = true;
    [SerializeField] private float cursorSpeed = 400f;

    [Header("Input Actions")]
    public InputAction cursorPositionAction;
    public InputAction placeAction;

    private Vector2 virtualCursorPosition;

    private void Awake()
    {
        building = transform.Find("BuildingGhost").GetComponent<BuildingGhost>();
    }

    private void OnEnable()
    {
        cursorPositionAction.Enable();
        placeAction.Enable();

        virtualCursorPosition = Mouse.current.position.ReadValue();
    }

    private void OnDisable()
    {
        cursorPositionAction.Disable();
        placeAction.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;

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

        UpdateCursorPosition();

        var mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(virtualCursorPosition.x, virtualCursorPosition.y, -mainCamera.transform.position.z));

        var cellPos = grid.IsPositionInsideGrid(mouseWorldPos) ? grid.GetCellPosition(mouseWorldPos) : mouseWorldPos;
        var canBuild = grid.CanPlaceBuilding(cellPos, currentBuildingData, Building.Rotation.Degree0);

        building.transform.position = cellPos;
        building.SetTint(canBuild ? validColor : invalidColor);

        var placeActionPressed = enableMouseControls ? Mouse.current.leftButton.wasPressedThisFrame : placeAction.WasPressedThisFrame();

        if (placeActionPressed && canBuild)
        {
            grid.AddBuilding(cellPos, currentBuildingData, Building.Rotation.Degree0);
        }
    }

    private void UpdateCursorPosition()
    {
        if (enableMouseControls)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            virtualCursorPosition = mousePos;
        }
        else
        {
            Vector2 moveInput = cursorPositionAction.ReadValue<Vector2>() * cursorSpeed;
            if (moveInput != Vector2.zero)
            {
                virtualCursorPosition += moveInput * Time.deltaTime;

                virtualCursorPosition.x = Mathf.Clamp(virtualCursorPosition.x, 0, Screen.width);
                virtualCursorPosition.y = Mathf.Clamp(virtualCursorPosition.y, 0, Screen.height);
            }
        }
    }
}
