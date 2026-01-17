using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    private record SpawnPoint
    {
        public Vector3 position;
        public Color color;
        public bool occupied;
    }

    private record JoinedPlayer
    {
        public int spawnPointIndex;
        public Gamepad gamepad;
        public PlayerInput playerInput;
        public int ID;
    }

    public bool active = false;

    [SerializeField] private PlayerRegistry playerRegistry;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private AudioSource playerSpawnSfx;
    [SerializeField] private Color[] spawnPointColors;

    private readonly List<JoinedPlayer> joinedPlayers = new();
    private SpawnPoint[] spawnPoints;

    private int playerLastID = 0;

    public Player[] GetPlayers()
    {
        return joinedPlayers.Select(joinedPlayer => joinedPlayer.playerInput.gameObject.GetComponent<Player>()).ToArray();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnLevelLoaded -= OnLevelLoaded;
    }

    private void Update()
    {
        if (!active) return;
        if (spawnPoints == null) return;

        foreach (var gamepad in Gamepad.all)
        {
            // gamepad already connected to player
            if (joinedPlayers.Any((player) => player.gamepad == gamepad))
                continue;

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                SpawnPlayer(gamepad);
            }
        }
    }

    private void SpawnPlayer(Gamepad gamepad)
    {
        if (playerSpawnSfx != null)
            playerSpawnSfx.Play();

        var playerInput = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamepad
        );
        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        playerInput.deviceLostEvent.AddListener(RemovePlayer);

        var joinedPlayer = new JoinedPlayer()
        {
            spawnPointIndex = GetNextFreeSpawnpoint(),
            gamepad = gamepad,
            playerInput = playerInput,
            ID = playerLastID++
        };
        var spawnPoint = spawnPoints[joinedPlayer.spawnPointIndex];

        var player = playerInput.GetComponent<Player>();
        player.OnFinishedRound += OnAnyPlayerEnterFinishArea;
        player.CallNextFrame(player.StartPlayingPhase, spawnPoint.position);
        player.gameObject.name = $"Player {joinedPlayer.ID} [{gamepad.device.displayName}]";
        player.SetColor(spawnPoint.color);

        joinedPlayers.Add(joinedPlayer);
        spawnPoint.occupied = true;

        playerRegistry.players.Add(player);

        Debug.Log($"Player {joinedPlayer.ID} joined: {joinedPlayer.gamepad.name}");
        EventBus.Instance?.OnPlayerJoined?.Invoke(player);
    }

    private void RemovePlayer(PlayerInput playerInput)
    {
        var joinedPlayer = joinedPlayers.Find((player) => player.playerInput == playerInput);

        joinedPlayers.Remove(joinedPlayer);
        spawnPoints[joinedPlayer.spawnPointIndex].occupied = false;

        Debug.Log($"Lost Player {joinedPlayer.ID}: {joinedPlayer.gamepad.name}");

        var player = playerInput.GetComponent<Player>();
        playerRegistry.players.Remove(player);

        EventBus.Instance?.OnPlayerLeft?.Invoke(player);

        this.CallNextFrame(Destroy, playerInput.gameObject);
    }

    private int GetNextFreeSpawnpoint()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var spawnpoint = spawnPoints[i];
            if (!spawnpoint.occupied)
            {
                return i;
            }
        }

        return 0;
    }

    private void OnLevelLoaded(Level level, bool isLobby)
    {
        active = isLobby || joinedPlayers.Count <= 0;

        spawnPoints = new SpawnPoint[level.SpawnPointsParent.childCount];
        for (int i = 0; i < level.SpawnPointsParent.childCount; i++)
        {
            spawnPoints[i] = new SpawnPoint()
            {
                position = level.SpawnPointsParent.GetChild(i).position,
                color = spawnPointColors[i],
                occupied = false
            };
        }

        foreach (var joinedPlayer in joinedPlayers)
        {
            var spawnPoint = spawnPoints[joinedPlayer.spawnPointIndex];
            spawnPoint.occupied = true;

            if (!active) continue; // only setup self not the players if not active

            var player = joinedPlayer.playerInput.GetComponent<Player>();
            player.OnFinishedRound += OnAnyPlayerEnterFinishArea;
            player.CallNextFrame(player.StartPlayingPhase, spawnPoint.position);
        }
    }

    private void OnAnyPlayerEnterFinishArea()
    {
        if (!active) return;

        // Disable self because player spawning during game is not intended
        active = false;

        // Unhook connected events
        foreach (var joinedPlayer in joinedPlayers)
        {
            var player = joinedPlayer.playerInput.GetComponent<Player>();
            player.OnFinishedRound -= OnAnyPlayerEnterFinishArea;
        }

        // Start game if any player enter the finish area
        EventBus.Instance?.OnStartGame?.Invoke();
    }

}
