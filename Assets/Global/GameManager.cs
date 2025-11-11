using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GamePhase
    {
        Building,
        Playing,
    }

    [SerializeField] private BuildGrid buildGrid;
    [SerializeField] private BuildingData[] buildings;

    [SerializeField] private Transform spawnPointsParent;

    private Player[] players;
    private GamePhase currentPhase;

    private void Start()
    {
        buildGrid.ShowGrid(false);
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
            player.OnPlacedBuilding += OnPlayerPlacesBuilding;
            player.OnFinishedRound += OnPlayerFinishedRound;
        }

        StartBuildingPhase();
    }

    private void StartBuildingPhase()
    {
        currentPhase = GamePhase.Building;

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

        // all players finsihed placing their building
        this.CallNextFrame(StartPlayingPhase);
    }

    private void StartPlayingPhase()
    {
        currentPhase = GamePhase.Playing;

        buildGrid.ShowGrid(false);

        for (int i = 0; i < players.Length; i++)
        {
            var spawnPosition = spawnPointsParent.GetChild(i).position;
            players[i].StartPlayingPhase(spawnPosition);
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

        // all players finsihed the round
        // TODO: check if max rounds are played
        this.CallNextFrame(StartBuildingPhase);
    }
}
