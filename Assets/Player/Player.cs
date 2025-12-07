using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool hasSelectedBuilding;
    public bool hasPlacedBuilding;
    public bool hasFinishedRound;

    public int numberOfSwingsThisRound;

    public Action OnSelectedBuilding;
    public Action OnPlacedBuilding;
    public Action OnFinishedRound;
    public Action<int> OnScoreChanges;
    public Action<string> OnSpecialShotAssigned;

    public int score;
    public List<int> scorePerRound = new();
    public float timeTookThisRound;

    [SerializeField] private GameObject confettiVFX;

    [SerializeField] private string buildingActionMapName = "Building";
    [SerializeField] private string playingActionMapName = "Playing";

    private PlayerInput playerInput;
    private PlayerBuildController buildController;
    private PlayerController playerController;

    private GameObject currentSpecialShotInstance;
    private Rigidbody2D playerControllerRigidbody;

    private Color color;
    private float startTime;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        buildController = transform.Find("PlayerBuilding").GetComponent<PlayerBuildController>();

        playerController = transform.Find("PlayerBall").GetComponent<PlayerController>();
        playerControllerRigidbody = playerController.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        buildController.gameObject.SetActive(false);
        playerController.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        buildController.OnSelectedBuilding += OnBuildingSelected;
        buildController.OnBuildingPlaced += OnBuildingPlaced;
        playerController.OnSwing += OnPlayerSwings;
    }

    private void OnDisable()
    {
        buildController.OnSelectedBuilding -= OnBuildingSelected;
        buildController.OnBuildingPlaced -= OnBuildingPlaced;
        playerController.OnSwing -= OnPlayerSwings;
    }

    public void ResetSelf()
    {
        playerController.ResetSelf();

        numberOfSwingsThisRound = 0;
        score = 0;
        scorePerRound.Clear();
    }

    public void StartNewRound()
    {
        hasPlacedBuilding = false;
        hasFinishedRound = true; // this means we are currently in building phase
        numberOfSwingsThisRound = 0;
    }

    public void StartSelectionPhase(Vector2 screenPosition)
    {
        playerInput.SwitchCurrentActionMap(buildingActionMapName);

        playerController.TogglePartyHat(false);
        playerController.gameObject.SetActive(false);

        hasSelectedBuilding = false;

        buildController.enabled = true;
        buildController.gameObject.SetActive(true);
        buildController.ToggleCursor(true);

        buildController.InitSelectionPhase(screenPosition);
    }

    public void StartBuildingPhase(BuildGrid buildGrid, BuildingData buildingData)
    {
        hasPlacedBuilding = false;

        buildController.gameObject.SetActive(true);
        buildController.ToggleCursor(true);

        buildController.InitBuildingPhase(buildGrid);
    }

    public void StartPlayingPhase(Vector3 spawnPosition)
    {
        playerInput.SwitchCurrentActionMap(playingActionMapName);

        buildController.gameObject.SetActive(false);

        hasFinishedRound = false;

        playerController.enabled = true;
        playerController.gameObject.SetActive(true);
        playerController.transform.position = spawnPosition;
        playerController.ResetSelf();

        StartTimer();

        numberOfSwingsThisRound = 0;

        playerController.SetSpecialShotAvailability(true);
        playerController.ResetSpecialShotEnabled();
    }

    // Generic way to assign a special shot to the player
    public void AssignSpecialShot(SpecialShotData specialShot)
    {
        if (currentSpecialShotInstance != null)
        {
            Destroy(currentSpecialShotInstance);
        }

        currentSpecialShotInstance = Instantiate(specialShot.prefab, playerController.transform);

        var specialShotComponent = currentSpecialShotInstance.GetComponent<SpecialShot>();
        specialShotComponent.Init(playerController, this, playerControllerRigidbody);

        OnSpecialShotAssigned?.Invoke(specialShot.name);
    }

    public void UsedSpecialShot()
    {
        playerController.SetSpecialShotAvailability(false);

        OnSpecialShotAssigned?.Invoke(""); // displays nothing in the UI
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        playerController.SetColor(color);
        buildController.SetColor(color);
    }

    public void AddScore(int scoreAwardedThisRound)
    {
        score += scoreAwardedThisRound;
        scorePerRound.Add(scoreAwardedThisRound);
        OnScoreChanges?.Invoke(score);
    }

    // is called via Unity's messaging system
    private void OnEnterFinishArea()
    {
        if (hasFinishedRound)
            return; // we are currently in build mode -> ignore event

        hasFinishedRound = true;
        StopTimer();

        playerController.CancelShotAndHideArrow();

        playerController.TogglePartyHat(true);
        playerController.enabled = false;

        var confetti = Instantiate(confettiVFX);
        confetti.transform.position = playerController.transform.position;

        OnFinishedRound?.Invoke();
    }

    private void OnBuildingSelected()
    {
        hasSelectedBuilding = true;
        buildController.gameObject.SetActive(false);
        buildController.ToggleCursor(false);

        OnSelectedBuilding?.Invoke();
    }

    private void OnBuildingPlaced()
    {
        hasPlacedBuilding = true;
        buildController.enabled = false;
        buildController.ToggleCursor(false);

        OnPlacedBuilding?.Invoke();
    }

    private void OnPlayerSwings()
    {
        numberOfSwingsThisRound++;
    }
    private void StartTimer()
    {
        startTime = Time.time;
    }
    private void StopTimer()
    {
        timeTookThisRound = Time.time - startTime;
    }
}
