using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildController : MonoBehaviour
{
    public BuildGrid grid;
    public BuildingData currentBuildingData;

    public Action OnSelectedBuilding;
    public Action OnBuildingPlaced;

    [SerializeField] private float cursorSpeed = 400f;
    [SerializeField] private float cursorRadius = 1f;
    [SerializeField] private float screenMargin = 20f;

    [SerializeField] private Color validColor = Color.white;
    [SerializeField] private Color invalidColor = Color.red;

    [SerializeField] private AudioSource selectSfx;
    [SerializeField] private AudioSource placeSfx;

    private Vector2 screenPos;

    private Vector2 moveInput;
    private bool placeInput;

    private SpriteRenderer cursor;
    private BuildingGhost buildingGhost;

    private bool IsInSelectionPhase => grid == null;

    private void Awake()
    {
        cursor = transform.Find("Cursor").GetComponent<SpriteRenderer>();
        buildingGhost = cursor.transform.Find("BuildingGhost").GetComponent<BuildingGhost>();
    }

    private void OnEnable()
    {
        ToggleCursor(true);
    }

    private void OnDisable()
    {
        ToggleCursor(false);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
        else
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }

    public void Place(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            if (IsInSelectionPhase)
            {
                if (currentBuildingData != null) return; // already selected a building

                var hits = Physics2D.OverlapCircleAll(cursor.transform.position, cursorRadius);
                foreach (var collider in hits)
                {
                    collider.gameObject.SendMessageUpwards("OnBuildingSelectedMessage", this, SendMessageOptions.DontRequireReceiver);
                }
            }
            else
            {
                if (currentBuildingData == null) return;

                placeInput = true;
            }
        }
    }

    public void RotateCW(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            var building = buildingGhost.currentBuilding;
            building.SetRotation(buildingGhost.data, building.GetNextRotation(true));
        }
    }

    public void RotateCCW(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            var building = buildingGhost.currentBuilding;
            building.SetRotation(buildingGhost.data, building.GetNextRotation(false));
        }
    }

    public void InitSelectionPhase(Vector2 screenPosition)
    {
        grid = null;
        screenPos = screenPosition;
    }

    public void InitBuildingPhase(BuildGrid buildGrid)
    {
        grid = buildGrid;

        buildingGhost.gameObject.SetActive(true);
        buildingGhost.ShowBuilding(currentBuildingData);
        buildingGhost.currentBuilding.transform.localScale = new Vector3(buildGrid.cellSize, buildGrid.cellSize, 1);

        screenPos = new Vector2(Screen.width * .5f, Screen.height * .5f);
    }

    public void SetColor(Color color)
    {
        cursor.material.color = color;
    }

    public void SetBuildingData(BuildingData buildingData)
    {
        if (selectSfx != null)
            selectSfx.Play();

        currentBuildingData = buildingData;
        OnSelectedBuilding?.Invoke();
    }

    public void ToggleCursor(bool enable)
    {
        cursor.gameObject.SetActive(enable);
    }

    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            screenPos += cursorSpeed * Time.deltaTime * moveInput;

            screenPos.x = Mathf.Clamp(screenPos.x, screenMargin, Screen.width - screenMargin);
            screenPos.y = Mathf.Clamp(screenPos.y, screenMargin, Screen.height - screenMargin);
        }

        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0;
        cursor.transform.position = worldPos;

        // try snapping buildingGhost to grid if we are in the building phase
        if (grid != null && currentBuildingData != null)
        {
            var cellPos = grid.IsPositionInsideGrid(worldPos) ? grid.GetCellPosition(worldPos) : worldPos;
            var canBuild = grid.CanPlaceBuilding(cellPos, currentBuildingData, buildingGhost.currentBuilding.rotation);

            cursor.transform.position = cellPos;
            buildingGhost.SetTint(canBuild ? validColor : invalidColor);

            if (canBuild && placeInput)
            {
                if (placeSfx != null)
                    placeSfx.Play();

                grid.AddBuilding(cellPos, currentBuildingData, buildingGhost.currentBuilding.rotation);
                OnBuildingPlaced?.Invoke();

                buildingGhost.gameObject.SetActive(false);
                currentBuildingData = null;
            }
        }

        // reset inputs
        placeInput = false;
    }
}