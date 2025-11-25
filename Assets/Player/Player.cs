using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    public bool hasPlacedBuilding;
    public bool hasFinishedRound;

    public int numberOfSwings;

    public Action OnPlacedBuilding;
    public Action OnFinishedRound;
    public Action<int> OnSwingsChanges;

    [SerializeField] private GameObject confettiVFX;

    [SerializeField] private string buildingActionMapName = "Building";
    [SerializeField] private string playingActionMapName = "Playing";

    private PlayerInput playerInput;
    private PlayerBuildController buildController;
    private PlayerController playerController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        buildController = transform.Find("PlayerBuilding").GetComponent<PlayerBuildController>();
        playerController = transform.Find("PlayerBall").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        buildController.OnBuildingPlaced += OnBuildingPlaced;
        playerController.OnSwing += OnPlayerSwings;
    }

    private void OnDisable()
    {
        buildController.OnBuildingPlaced -= OnBuildingPlaced;
        playerController.OnSwing -= OnPlayerSwings;
    }

    public void StartNewRound()
    {
        hasPlacedBuilding = false;
        hasFinishedRound = true; // this means we are currently in building phase
        numberOfSwings = 0;
        OnSwingsChanges?.Invoke(numberOfSwings);
    }

    public void StartBuildingPhase(BuildGrid buildGrid, BuildingData buildingData)
    {
        playerInput.SwitchCurrentActionMap(buildingActionMapName);

        playerController.TogglePartyHat(false);
        playerController.gameObject.SetActive(false);

        hasPlacedBuilding = false;

        buildController.enabled = true;
        buildController.gameObject.SetActive(true);
        buildController.Init(buildGrid, buildingData);
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
        return playerController.GetComponent<Renderer>().material.color;
    }

    public void SetColor(Color color)
    {
        playerController.GetComponent<Renderer>().material.color = color;
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

    private void OnBuildingPlaced()
    {
        hasPlacedBuilding = true;
        buildController.enabled = false;

        OnPlacedBuilding?.Invoke();
    }

    private void OnPlayerSwings()
    {
        numberOfSwings++;
        OnSwingsChanges?.Invoke(numberOfSwings);
    }
}
