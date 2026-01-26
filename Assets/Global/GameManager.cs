using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private enum GamePhase
    {
        Selection,
        Building,
        Playing,
    }

    const int BASE_LEVEL_SCENE_INDEX = 0;

    [SerializeField] private PlayerRegistry playerRegistry;

    [SerializeField] private float screenBorderDistance;

    [Header("Game Settings")]
    [SerializeField] private int maxRoundsPerGame = 6;

    [SerializeField] private BuildingData[] buildings;
    [SerializeField] private SpecialShotData[] specialShots;

    [SerializeField] private int pointsForWinningRound = 25;
    [SerializeField] private int pointsDeductedPerPlacement = 5;
    [SerializeField] private int bonusPointsForFastestPlayer = 10;

    private GamePhase currentPhase;

    private Level currentLevel;
    private int roundCount;

    private void Awake()
    {
        roundCount = 0;
    }

    private void OnEnable()
    {
        EventBus.Instance.OnSwitchToScene += OnSwitchToScene;
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnStartGame += StartGame;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnSwitchToScene -= OnSwitchToScene;
        EventBus.Instance.OnLevelLoaded -= OnLevelLoaded;
        EventBus.Instance.OnStartGame -= StartGame;
    }

    private void OnLevelLoaded(Level level, bool isLobby)
    {
        Debug.Log($"Loaded '{level.name}' with {playerRegistry.players.Count} {(playerRegistry.players.Count == 1 ? "player" : "players")}!");
        currentLevel = level;

        roundCount = 0;

        currentLevel.BuildGrid.ShowGrid(false);
        currentLevel.BuildingSpawner.gameObject.SetActive(false);

        foreach (var player in playerRegistry.players)
        {
            player.ResetSelf();
        }

        if (!isLobby && playerRegistry.players.Count > 0)
        {
            this.CallNextFrame(StartGame);
        }
    }

    private void StartGame()
    {
        Debug.Log($"Starting game with {playerRegistry.players.Count} {(playerRegistry.players.Count == 1 ? "player" : "players")}!");
        foreach (var player in playerRegistry.players)
        {
            player.OnSelectedBuilding += OnPlayerSelectsBuilding;
            player.OnPlacedBuilding += OnPlayerPlacesBuilding;
            player.OnFinishedRound += OnPlayerFinishedRound;

            player.StartNewRound();
        }

        StartBuildingSelectionPhase();
    }

    private void StartBuildingSelectionPhase()
    {
        currentPhase = GamePhase.Selection;

        // selection phase begins a new round
        roundCount++;
        EventBus.Instance?.OnRoundStart?.Invoke(maxRoundsPerGame, roundCount);

        currentLevel.BuildingSpawner.gameObject.SetActive(true);
        currentLevel.BuildingSpawner.SpawnBuildings(buildings, playerRegistry.players.Count + 1, currentLevel.BuildGrid.GetOccupationPercentage());

        for (int i = 0; i < playerRegistry.players.Count; i++)
        {
            var positions = new Vector3[]
            {
                new(screenBorderDistance, screenBorderDistance, 0),
                new(screenBorderDistance, Screen.height - screenBorderDistance, 0),
                new(Screen.width - screenBorderDistance, Screen.height - screenBorderDistance, 0),
                new(Screen.width - screenBorderDistance, 0, 0)
            };
            playerRegistry.players[i].StartSelectionPhase(positions[i]);
        }
    }

    private void OnPlayerSelectsBuilding()
    {
        if (currentPhase != GamePhase.Selection)
            return;

        foreach (var player in playerRegistry.players)
        {
            if (!player.hasSelectedBuilding)
                return;
        }

        // all players finished selecting their building
        this.CallNextFrame(StartBuildingPhase);
    }

    private void StartBuildingPhase()
    {
        currentPhase = GamePhase.Building;

        currentLevel.BuildingSpawner.gameObject.SetActive(false);

        currentLevel.BuildGrid.ShowGrid(true);

        foreach (var player in playerRegistry.players)
        {
            player.StartBuildingPhase(currentLevel.BuildGrid, buildings[Random.Range(0, buildings.Length)]);
        }
    }

    private void OnPlayerPlacesBuilding()
    {
        if (currentPhase != GamePhase.Building)
            return;

        foreach (var player in playerRegistry.players)
        {
            if (!player.hasPlacedBuilding)
                return;
        }

        // all players finished placing their building
        this.CallNextFrame(StartPlayingPhase);
    }

    private void StartPlayingPhase()
    {
        currentPhase = GamePhase.Playing;

        currentLevel.BuildGrid.ShowGrid(false);

        var specialShotForRound = specialShots[Random.Range(0, specialShots.Length)];

        for (int i = 0; i < playerRegistry.players.Count; i++)
        {
            var spawnPosition = currentLevel.SpawnPointsParent.GetChild(i).position;
            playerRegistry.players[i].StartPlayingPhase(spawnPosition);

            playerRegistry.players[i].AssignSpecialShot(specialShotForRound);
        }
    }

    private void OnPlayerFinishedRound()
    {
        if (currentPhase != GamePhase.Playing)
            return;

        foreach (var player in playerRegistry.players)
        {
            if (!player.hasFinishedRound)
                return;
        }

        // all players finished the round
        AwardScore();

        // disable UI hints
        EventBus.Instance?.OnToggleSurrenderHint?.Invoke(false);

        if (roundCount >= maxRoundsPerGame)
        {
            this.CallNextFrame(OnGameOver);
        }
        else
        {
            this.CallNextFrame(StartBuildingSelectionPhase);
        }
    }

    private void OnGameOver()
    {
        // clean up event subscriptions
        foreach (var player in playerRegistry.players)
        {
            player.OnSelectedBuilding -= OnPlayerSelectsBuilding;
            player.OnPlacedBuilding -= OnPlayerPlacesBuilding;
            player.OnFinishedRound -= OnPlayerFinishedRound;
        }

        // declare winner
        var winner = playerRegistry.players[0];
        for (int i = 1; i < playerRegistry.players.Count; i++)
        {
            if (playerRegistry.players[i].score > winner.score)
            {
                winner = playerRegistry.players[i];
            }
        }
        EventBus.Instance?.OnWinnerDecided?.Invoke(winner, playerRegistry.players.ToArray(), maxRoundsPerGame);
    }

    private async void OnSwitchToScene(int buildIndex)
    {
        var unloadOperations = new List<AsyncOperation>();

        // unload all the scenes besides the base scene
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);

            if (scene.buildIndex == BASE_LEVEL_SCENE_INDEX) continue;

            unloadOperations.Add(SceneManager.UnloadSceneAsync(scene));
        }

        // wait until all scenes are unloaded
        foreach (var unloadOperation in unloadOperations)
        {
            await unloadOperation;
        }

        await Awaitable.NextFrameAsync();
        await SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
    }

    private void AwardScore()
    {
        var fastestTime = playerRegistry.players.Min(player => player.timeTookThisRound);

        playerRegistry.players = playerRegistry.players.OrderBy(player => player.numberOfSwingsThisRound).ToList();
        int currentPlacement = 0;
        int currentSwings = playerRegistry.players[0].numberOfSwingsThisRound;

        for (int i = 0; i < playerRegistry.players.Count; i++)
        {
            if (currentSwings != playerRegistry.players[i].numberOfSwingsThisRound)
            {
                currentPlacement++;
            }
            int pointsAwarded = pointsForWinningRound - (currentPlacement * pointsDeductedPerPlacement);

            if (playerRegistry.players[i].timeTookThisRound == fastestTime)
            {
                pointsAwarded += bonusPointsForFastestPlayer;
            }
            playerRegistry.players[i].AddScore(pointsAwarded);
        }
    }
}