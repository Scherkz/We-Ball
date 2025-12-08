using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Sprite playerCursorPrefab;

    private readonly List<LobbyPlayer> players = new();
    private Dictionary<LobbyPlayer, GameObject> voteIcons = new();
    private PlayerSpawner playerSpawner;

    public record LobbyPlayer
    {
        public Player player;
        public int playerID;
        public MapNode mapVote;
    }

    private void OnEnable()
    {
        if (EventBus.Instance == null) return;
        EventBus.Instance.OnPlayerJoined += OnPlayerJoined;
        EventBus.Instance.OnPlayerLeft += OnPlayerLeft;
        EventBus.Instance.OnMapVoted += OnMapVoted;
    }

    private void OnDisable()
    {
        if (EventBus.Instance == null) return;
        EventBus.Instance.OnPlayerJoined -= OnPlayerJoined;
        EventBus.Instance.OnPlayerLeft -= OnPlayerLeft;
        EventBus.Instance.OnMapVoted -= OnMapVoted;
    }

    private void Start()
    {
        if(playerSpawner == null) 
        { 
            playerSpawner = FindFirstObjectByType<PlayerSpawner>(); 
        }
        Debug.Log(playerSpawner);

        playerSpawner.allowJoining = true;
    }

    private void OnPlayerJoined(Player player)
    {
        RegisterPlayer(player);
    }

    private void OnPlayerLeft(Player player)
    {
        var lobbyPlayer = players.Find(lp => lp.player == player);
        if (lobbyPlayer != null)
        {
            players.Remove(lobbyPlayer);
        }
    }

    private void RegisterPlayer(Player player)
    {
        if (players.Any(lobbyPlayer => lobbyPlayer.player == player)) return;

        var lobbyPlayer = new LobbyPlayer()
        {
            player = player,
            playerID = players.Count(),
            mapVote = null
    };

        players.Add(lobbyPlayer);

        Debug.Log($"Player {lobbyPlayer.playerID} registered.");
    }

    private void OnMapVoted(MapNode map, Player player)
    {
        var lobbyPlayer = players.Find(lobbyPlayer => lobbyPlayer.player == player);
        
        if (lobbyPlayer == null) return;

        MapNode previousVote = lobbyPlayer.mapVote;

        lobbyPlayer.mapVote = map;

        GameObject icon;

        if (voteIcons.ContainsKey(lobbyPlayer))
        {
            icon = voteIcons[lobbyPlayer];
            icon.transform.SetParent(map.voteAnchor, false);
        }
        else
        {
            icon = new GameObject($"VoteIcon_{lobbyPlayer.playerID}");
            var spriteRenderer = icon.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = playerCursorPrefab;
            spriteRenderer.color = lobbyPlayer.player.GetColor();
            spriteRenderer.sortingOrder = 10;
            icon.transform.SetParent(map.voteAnchor, false);
            icon.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            voteIcons[lobbyPlayer] = icon;
        }

        if (previousVote != null)
            RebuildVoteIcons(previousVote);

        RebuildVoteIcons(map);

        Debug.Log($"Player {lobbyPlayer.playerID} map vote: {lobbyPlayer.mapVote.mapName}");

        TryStartingLevel();
    }

    private void RebuildVoteIcons(MapNode map)
    {
        var icons = voteIcons
            .Where(mapVote => mapVote.Key.mapVote == map)
            .Select(mapVote => mapVote.Value)
            .ToList();

        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].transform.localPosition = new Vector3(i * 0.5f, 0 , 0);
        }
    }

    private void TryStartingLevel()
    {
        if (!players.Any()) return;

        if (players.Any(player => player.mapVote == null))
            return;

        MapNode winning = PickWinningMap(players.Select(player => player.mapVote).ToList());
        playerSpawner.allowJoining = false;

        Debug.Log($"LobbyManager selected map: {winning.mapName}");
        EventBus.Instance?.OnMapSelected?.Invoke(winning, 10);
    }

    private MapNode PickWinningMap(List<MapNode> maps)
    {
        Dictionary<MapNode, int> voteCounts = new Dictionary<MapNode, int>();
        List<MapNode> topMaps = new List<MapNode>();
        int maxVotes = 0;

        foreach (var map in maps)
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

    public LobbyPlayer GetLobbyPlayer(Player player)
    {
        return players.Find(lobbyPlayer => lobbyPlayer.player == player);
    }

}
