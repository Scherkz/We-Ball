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
    private PlayerIndicator playerIndicator;

    private GameObject currentSpecialShotInstance;
    private Rigidbody2D playerControllerRigidbody;

    private Color color;
    private float startTime;
    private Vector3 spawnPoint;
    
    private enum RoundFinishReason
    {
        ReachedFinish,
        Surrender
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        buildController = transform.Find("PlayerBuilding").GetComponent<PlayerBuildController>();

        playerController = transform.Find("PlayerBall").GetComponent<PlayerController>();
        playerControllerRigidbody = playerController.GetComponent<Rigidbody2D>();

        playerIndicator = transform.Find("PlayerIndicator").GetComponent<PlayerIndicator>();
    }

    private void Start()
    {
        buildController.enabled = false;
        playerController.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        buildController.OnSelectedBuilding += OnBuildingSelected;
        buildController.OnBuildingPlaced += OnBuildingPlaced;
        playerController.OnSwing += OnPlayerSwings;
        playerController.onSurrenderConfirmed += OnPlayerSurrendered;
    }

    private void OnDisable()
    {
        buildController.OnSelectedBuilding -= OnBuildingSelected;
        buildController.OnBuildingPlaced -= OnBuildingPlaced;
        playerController.OnSwing -= OnPlayerSwings;
        playerController.onSurrenderConfirmed -= OnPlayerSurrendered;
    }

    public void StartNewRound()
    {
        playerController.ResetSelf();

        numberOfSwingsThisRound = 0;

        hasPlacedBuilding = false;
        hasFinishedRound = true; // this means we are currently in building phase
    }

    public void ResetSelf()
    {
        playerController.ResetSelf();

        numberOfSwingsThisRound = 0;
        score = 0;
        scorePerRound.Clear();
    }

    public void StartSelectionPhase(Vector2 screenPosition)
    {
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap(buildingActionMapName);

        playerController.TogglePartyHat(false);
        playerController.gameObject.SetActive(false);

        hasSelectedBuilding = false;

        buildController.enabled = true;
        buildController.InitSelectionPhase(screenPosition);
    }

    public void StartBuildingPhase(BuildGrid buildGrid, BuildingData buildingData)
    {
        playerInput.ActivateInput();
        
        hasPlacedBuilding = false;

        buildController.enabled = true;
        buildController.InitBuildingPhase(buildGrid);

        playerController.ResetSpecialShotSpecifics();
    }

    public void StartPlayingPhase(Vector3 spawnPosition)
    {
        playerInput.ActivateInput();
        playerInput.SwitchCurrentActionMap(playingActionMapName);

        buildController.enabled = false;

        hasFinishedRound = false;

        playerController.enabled = true;
        playerController.gameObject.SetActive(true);
        playerController.transform.position = spawnPosition;
        playerController.ResetSelf();

        StartTimer();

        numberOfSwingsThisRound = 0;
        spawnPoint = spawnPosition;

        playerController.SetSpecialShotAvailability(true);
        playerController.DisableSpecialShot();
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
        playerController.OnToggleSpecialShotVFX?.Invoke(false);
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
        playerIndicator.SetColor(color);
    }

    public void AddScore(int scoreAwardedThisRound)
    {
        score += scoreAwardedThisRound;
        scorePerRound.Add(scoreAwardedThisRound);
        OnScoreChanges?.Invoke(score);
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    private void FinishRound(RoundFinishReason reason)
    {
        if (hasFinishedRound)
            return; // we are currently in build mode -> ignore event

        hasFinishedRound = true;
        StopTimer();

        playerController.CancelShotAndHideArrow();
        playerInput.DeactivateInput();
        
        playerController.TogglePartyHat(true);
        playerController.enabled = false;

        switch (reason)
        {
            case RoundFinishReason.Surrender:
                numberOfSwingsThisRound = int.MaxValue;
                timeTookThisRound = float.MaxValue;
                break;
            
            case RoundFinishReason.ReachedFinish:
                var confetti = Instantiate(confettiVFX);
                confetti.transform.position = playerController.transform.position;
                break;
        }

        OnFinishedRound?.Invoke();
    }
    
    // is called via Unity's messaging system
    private void OnEnterFinishArea()
    {
        FinishRound(RoundFinishReason.ReachedFinish);
    }

    // is called via Unity's messaging system
    private void OnEnterKillAreaMessage()
    {
        playerController.transform.position = spawnPoint;
        playerController.ResetSelf();
    }

    // is called via Unity's messaging system through MapNode.cs
    private void OnMapNodeVotedMessage(MapNode map)
    {
        // Invoke map vote through event bus
        EventBus.Instance?.OnMapVoted?.Invoke(map, this);
    }

    private void OnBuildingSelected()
    {
        hasSelectedBuilding = true;
        buildController.enabled = false;
        
        playerInput.ActivateInput();

        OnSelectedBuilding?.Invoke();
    }

    private void OnBuildingPlaced()
    {
        hasPlacedBuilding = true;
        buildController.enabled = false;
        
        playerInput.DeactivateInput();

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

    // is called via Unity's messaging system
    private void ApplyForceImpulseMessage(Vector2 impulse)
    {
        if (playerControllerRigidbody == null)
            return;

        playerControllerRigidbody.AddForce(impulse, ForceMode2D.Impulse);
    }

    private void OnPlayerSurrendered()
    {
        FinishRound(RoundFinishReason.Surrender);
    }
}
