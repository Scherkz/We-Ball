using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private record LobbyPlayer
    {
        public Player player;
        public int playerID;
        public MapNode mapVote;
    }

    [SerializeField] private int gameStartCountdownSeconds = 5;

    [SerializeField] private Sprite playerCursorPrefab;

    private readonly List<LobbyPlayer> players = new();
    private PlayerSpawner playerSpawner;
    private bool countdownActive = false;

    private void OnEnable()
    {
        if (EventBus.Instance == null) return;
        EventBus.Instance.OnPlayerJoined += RegisterPlayer;
        EventBus.Instance.OnPlayerLeft += RemovePlayer;
        EventBus.Instance.OnMapVoted += OnMapVoted;
        EventBus.Instance.OnMapSelected += OnMapSelected;
    }

    private void OnDisable()
    {
        if (EventBus.Instance == null) return;
        EventBus.Instance.OnPlayerJoined -= RegisterPlayer;
        EventBus.Instance.OnPlayerLeft -= RemovePlayer;
        EventBus.Instance.OnMapVoted -= OnMapVoted;
        EventBus.Instance.OnMapSelected -= OnMapSelected;
    }

    private void Start()
    {
        if (playerSpawner == null)
        {
            playerSpawner = FindFirstObjectByType<PlayerSpawner>();
        }

        // reconstruct lobby players from playerSpawner
        foreach (var player in playerSpawner.GetPlayers())
        {
            var lobbyPlayer = new LobbyPlayer()
            {
                player = player,
                playerID = players.Count(),
                mapVote = null
            };
            players.Add(lobbyPlayer);
        }

        playerSpawner.active = true;
    }

    private void RegisterPlayer(Player player)
    {
        if (players.Any(lobbyPlayer => lobbyPlayer.player == player) || countdownActive) return;

        var lobbyPlayer = new LobbyPlayer()
        {
            player = player,
            playerID = players.Count(),
            mapVote = null
        };

        players.Add(lobbyPlayer);

        Debug.Log($"Player {lobbyPlayer.playerID} registered.");
    }

    private void RemovePlayer(Player player)
    {
        var lobbyPlayer = players.Find(lp => lp.player == player);
        if (lobbyPlayer != null)
        {
            if (lobbyPlayer.mapVote != null)
                lobbyPlayer.mapVote.RemoveVote(lobbyPlayer.playerID);

            players.Remove(lobbyPlayer);
        }
    }

    private void OnMapVoted(MapNode map, Player player)
    {
        if (countdownActive) return;

        var lobbyPlayer = players.Find(lobbyPlayer => lobbyPlayer.player == player);

        if (lobbyPlayer == null) return;

        MapNode previousVote = lobbyPlayer.mapVote;
        lobbyPlayer.mapVote = map;

        if (previousVote != null)
        {
            previousVote.RemoveVote(lobbyPlayer.playerID);
        }

        map.AddVote(lobbyPlayer.playerID, lobbyPlayer.player.GetColor());

        Debug.Log($"Player {lobbyPlayer.playerID} map vote: {lobbyPlayer.mapVote.mapName}");

        TryStartingLevel();
    }

    private void TryStartingLevel()
    {
        if (!players.Any() || players.Any(player => player.mapVote == null) || countdownActive) return;

        playerSpawner.active = false;
        countdownActive = true;

        MapNode winning = PickWinningMap(players.Select(player => player.mapVote).ToList());

        Debug.Log($"LobbyManager selected map: {winning.mapName}");
        EventBus.Instance?.OnMapSelected?.Invoke(winning, gameStartCountdownSeconds);
    }

    private MapNode PickWinningMap(List<MapNode> votedMaps)
    {
        Dictionary<MapNode, int> voteCounts = new Dictionary<MapNode, int>();
        List<MapNode> topMaps = new List<MapNode>();
        int maxVotes = 0;

        foreach (var map in votedMaps)
        {
            if (!voteCounts.ContainsKey(map))
                voteCounts[map] = 0;

            voteCounts[map]++;

            if (voteCounts[map] > maxVotes)
            {
                maxVotes = voteCounts[map];
                topMaps.Clear();
                topMaps.Add(map);
            }
            else if (voteCounts[map] == maxVotes && !topMaps.Contains(map))
            {
                topMaps.Add(map);
            }
        }

        int randomMapIndex = Random.Range(0, topMaps.Count);
        return topMaps[randomMapIndex];
    }

    private void OnMapSelected(MapNode map, int countdown)
    {
        StartCoroutine(CountdownRoutine(map, countdown));
    }

    private IEnumerator CountdownRoutine(MapNode map, int countdown)
    {
        yield return new WaitForSeconds(countdown);
        
        EventBus.Instance.OnSwitchToScene?.Invoke(map.sceneBuildIndex);
    }
}
