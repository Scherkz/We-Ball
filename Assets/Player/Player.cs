using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool hasPlacedBuilding;
    public bool hasFinishedRound;

    public Action OnPlacedBuilding;
    public Action OnFinishedRound;

    private PlayerBuildController buildController;
    private PlayerController playerController;

    private void Awake()
    {
        buildController = transform.Find("PlayerBuilding").GetComponent<PlayerBuildController>();
        playerController = transform.Find("PlayerBall").GetComponent<PlayerController>();

        buildController.gameObject.SetActive(false);
        playerController.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        buildController.OnBuildingPlaced += OnBuildingPlaced;
    }

    private void OnDisable()
    {
        buildController.OnBuildingPlaced -= OnBuildingPlaced;
    }

    public void StartBuildingPhase(BuildGrid buildGrid, BuildingData buildingData)
    {
        playerController.gameObject.SetActive(false);

        hasPlacedBuilding = false;

        buildController.enabled = true;
        buildController.gameObject.SetActive(true);
        buildController.Init(buildGrid, buildingData);
    }

    public void StartPlayingPhase(Vector3 spawnPosition)
    {
        buildController.gameObject.SetActive(false);

        hasFinishedRound = false;

        playerController.enabled = true;
        playerController.gameObject.SetActive(true);
        playerController.transform.position = spawnPosition;
    }

    private void OnEnterFinishArea()
    {
        hasFinishedRound = true;
        playerController.enabled = false;

        OnFinishedRound?.Invoke();
    }

    private void OnBuildingPlaced()
    {
        hasPlacedBuilding = true;
        buildController.enabled = false;

        OnPlacedBuilding?.Invoke();
    }
}
