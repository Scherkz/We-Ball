using NUnit.Framework;
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
    public Action<int> OnSwingsChanges;
    public Action<int> OnScoreChanges;

    public int score;
    public List<int> scorePerRound = new List<int>();

    [SerializeField] private GameObject confettiVFX;

    [SerializeField] private string buildingActionMapName = "Building";
    [SerializeField] private string playingActionMapName = "Playing";

    private PlayerInput playerInput;
    private PlayerBuildController buildController;
    private PlayerController playerController;
    
    private Color color;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        buildController = transform.Find("PlayerBuilding").GetComponent<PlayerBuildController>();
        playerController = transform.Find("PlayerBall").GetComponent<PlayerController>();
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

    public void StartNewRound()
    {
        hasPlacedBuilding = false;
        hasFinishedRound = true; // this means we are currently in building phase
        numberOfSwingsThisRound = 0;
        OnSwingsChanges?.Invoke(numberOfSwingsThisRound);
    }

    public void StartSelectionPhase(Vector2 screenPosition)
    {
        playerInput.SwitchCurrentActionMap(buildingActionMapName);
        
        playerController.TogglePartyHat(false);
        playerController.gameObject.SetActive(false);

        hasSelectedBuilding = false;
        
        buildController.enabled = true;
        buildController.gameObject.SetActive(true);
        
        buildController.InitSelectionPhase(screenPosition);
    }

    public void StartBuildingPhase(BuildGrid buildGrid, BuildingData buildingData)
    {
        hasPlacedBuilding = false;
        
        buildController.gameObject.SetActive(true);

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

    // is called via Unity's messaging system
    private void OnEnterFinishArea()
    {
        if (hasFinishedRound)
            return; // we are currently in build mode -> ignore event

        hasFinishedRound = true;
        
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
        
        OnSelectedBuilding?.Invoke();
    }

    private void OnBuildingPlaced()
    {
        hasPlacedBuilding = true;
        buildController.enabled = false;

        OnPlacedBuilding?.Invoke();
    }

    private void OnPlayerSwings()
    {
        numberOfSwingsThisRound++;
        OnSwingsChanges?.Invoke(numberOfSwingsThisRound);
    }
}
