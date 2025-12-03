using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GamePhase
    {
        Selection,
        Building,
        Playing,
    }

    [SerializeField] private int maxRoundsPerGame = 6;

    [SerializeField] private BuildingSpawner buildingSpawner;
    [SerializeField] private BuildGrid buildGrid;
    [SerializeField] private BuildingData[] buildings;
    [SerializeField] private SpecialShotData[] specialShots;

    [SerializeField] private Transform spawnPointsParent;

    [SerializeField] private float screenBorderDistance;

    [SerializeField] private int pointsForWinningRound = 25;
    [SerializeField] private int pointsDeductedPerAdditionalShot = 5;
    [SerializeField] private int bonusPointsForFastestPlayer = 10;

    private Player[] players;
    private GamePhase currentPhase;

    private int roundCount;

    private void Awake()
    {
        roundCount = 0;
    }

    private void Start()
    {
        buildGrid.ShowGrid(false);
        buildingSpawner.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnStartGame += StartRound;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnStartGame -= StartRound;
    }

    public void StartRound(Player[] players)
    {
        Debug.Log($"Starting game with {players.Length} {(players.Length == 1 ? "player" : "players")}!");
        this.players = players;

        foreach (var player in players)
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

        buildingSpawner.gameObject.SetActive(true);
        buildingSpawner.SpawnBuildings(buildings, players.Length + 1);

        for (int i = 0; i < players.Length; i++)
        {
            var positions = new Vector3[]
            {
                new Vector3(screenBorderDistance, screenBorderDistance, 0),
                new Vector3(screenBorderDistance, Screen.height - screenBorderDistance, 0),
                new Vector3(Screen.width - screenBorderDistance, Screen.height - screenBorderDistance, 0),
                new Vector3(Screen.width - screenBorderDistance, 0, 0)
            };
            players[i].StartSelectionPhase(positions[i]);
        }
    }

    private void OnPlayerSelectsBuilding()
    {
        if (currentPhase != GamePhase.Selection)
            return;

        foreach (var player in players)
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

        buildingSpawner.gameObject.SetActive(false);

        buildGrid.ShowGrid(true);

        foreach (var player in players)
        {
            player.StartBuildingPhase(buildGrid, buildings[Random.Range(0, buildings.Length)]);
        }
    }

    private void OnPlayerPlacesBuilding()
    {
        if (currentPhase != GamePhase.Building)
            return;

        foreach (var player in players)
        {
            if (!player.hasPlacedBuilding)
                return;
        }

        // all players finished placing their building
        this.CallNextFrame(StartPlayingPhase);
    }

    private SpecialShotData GetRandomSpecialShot()
    {
        return specialShots[Random.Range(0, specialShots.Length)];
    }

    private void StartPlayingPhase()
    {
        currentPhase = GamePhase.Playing;

        buildGrid.ShowGrid(false);

        var specialShotForRound = GetRandomSpecialShot();

        for (int i = 0; i < players.Length; i++)
        {
            var spawnPosition = spawnPointsParent.GetChild(i).position;
            players[i].StartPlayingPhase(spawnPosition);

            players[i].AssignSpecialShot(specialShotForRound);
        }
    }

    private void OnPlayerFinishedRound()
    {
        if (currentPhase != GamePhase.Playing)
            return;

        foreach (var player in players)
        {
            if (!player.hasFinishedRound)
                return;
        }

        // all players finished the round
        int leastSwings = players.Min(player => player.numberOfSwingsThisRound);

        var fastestPlayer = players.OrderBy(player => player.timeTookThisRound).First();

        foreach (var player in players)
        {
            var additionalSwings = player.numberOfSwingsThisRound - leastSwings;
            var scoreAwardedThisRound = Mathf.Max(0, pointsForWinningRound - (additionalSwings * pointsDeductedPerAdditionalShot));
            if (player == fastestPlayer)
            {
                scoreAwardedThisRound += bonusPointsForFastestPlayer;
            }
            player.AddScore(scoreAwardedThisRound);
        }

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
        foreach (var player in players)
        {
            player.OnPlacedBuilding -= OnPlayerPlacesBuilding;
            player.OnFinishedRound -= OnPlayerFinishedRound;
        }

        // declare winner
        var winner = players[0];
        for (int i = 1; i < players.Length; i++)
        {
            if (players[i].score > winner.score)
            {
                winner = players[i];
            }
        }
        EventBus.Instance?.OnWinnerDicided?.Invoke(winner, players, maxRoundsPerGame);
    }
}