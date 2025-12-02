using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LobbyManager;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField] private GameObject playerBoardEntryPrefab;
    private readonly Dictionary<Player, PlayerBoardEntry> entries = new();
    private LobbyManager lobbyManager = null;

    private void OnEnable()
    {
        EventBus.Instance.OnPlayerJoined += AddPlayerEntry;
        EventBus.Instance.OnPlayerLeft += RemovePlayerEntry;
        EventBus.Instance.OnMapVoted += OnPlayerVoted;
        EventBus.Instance.OnPlayerReady += OnPlayerReady;
    }

    private void OnDisable()
    {
        if (EventBus.Instance == null) return;

        EventBus.Instance.OnPlayerJoined -= AddPlayerEntry;
        EventBus.Instance.OnPlayerLeft -= RemovePlayerEntry;
        EventBus.Instance.OnMapVoted -= OnPlayerVoted;
        EventBus.Instance.OnPlayerReady -= OnPlayerReady;
    }

    private void Start()
    {
        var players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        lobbyManager = FindFirstObjectByType<LobbyManager>();

        foreach (var player in players)
        {
            AddPlayerEntry(player);
        }
    }

    private void AddPlayerEntry(Player player)
    {
        if (entries.ContainsKey(player)) return;

        var entryPrefab = Instantiate(playerBoardEntryPrefab, transform);
        var entry = entryPrefab.GetComponent<PlayerBoardEntry>();

        entries[player] = entry;

        var lobbyPlayer = lobbyManager.GetLobbyPlayer(player);
        entry.SetPlayerColor(lobbyPlayer.player.GetColor());
        entry.SetPlayerName(lobbyPlayer.playerID);
        entry.SetReady(lobbyPlayer.ready);
        entry.SetMapVote(lobbyPlayer.mapVote?.mapName);

        RebuildEntryPositions();
    }

    private void RemovePlayerEntry(Player player)
    {
        if (!entries.ContainsKey(player)) return;

        Destroy(entries[player].gameObject);
        entries.Remove(player);

        RebuildEntryPositions();
    }

    private void OnPlayerVoted(MapNode map, Player player)
    {
        if (!entries.ContainsKey(player)) return;

        entries[player].SetMapVote(map.mapName);
    }

    private void OnPlayerReady(Player player)
    {
        if (!entries.ContainsKey(player)) return;

        var lobbyPlayer = lobbyManager.GetLobbyPlayer(player);
        entries[player].SetReady(lobbyPlayer.ready);
    }

    private void RebuildEntryPositions()
    {
        int index = 0;

        foreach (var pair in entries.OrderBy(entry => lobbyManager.GetLobbyPlayer(entry.Key).playerID))
        {
            var entry = pair.Value;
            var transform = entry.GetComponent<RectTransform>();

            transform.anchoredPosition = new Vector2(0, -index * 32f);

            var lobbyPlayer = lobbyManager.GetLobbyPlayer(pair.Key);
            lobbyPlayer.playerID = index;

            entry.SetPlayerName(index);

            index++;
        }
    }


}
