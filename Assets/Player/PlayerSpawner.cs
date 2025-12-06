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
        public SpawnPoint spawnpoint;
        public Gamepad gamepad;
        public PlayerInput playerInput;
        public int ID;
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Color[] spawnPointColors;

    private readonly List<JoinedPlayer> joinedPlayers = new();
    private SpawnPoint[] spawnPoints;

    private int playerLastID = 0;

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
        var playerInput = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamepad
        );
        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        playerInput.deviceLostEvent.AddListener(RemovePlayer);

        var joinedPlayer = new JoinedPlayer()
        {
            spawnpoint = GetNextFreeSpawnpoint(),
            gamepad = gamepad,
            playerInput = playerInput,
            ID = playerLastID++
        };

        var player = playerInput.GetComponent<Player>();
        player.OnFinishedRound += OnAnyPlayerEnterFinishArea;
        player.CallNextFrame(player.StartPlayingPhase, joinedPlayer.spawnpoint.position);
        player.gameObject.name = $"Player {joinedPlayer.ID} [{gamepad.device.displayName}]";
        player.SetColor(joinedPlayer.spawnpoint.color);

        joinedPlayers.Add(joinedPlayer);
        joinedPlayer.spawnpoint.occupied = true;

        Debug.Log($"Player {joinedPlayer.ID} joined: {joinedPlayer.gamepad.name}");
    }

    private void RemovePlayer(PlayerInput playerInput)
    {
        var joinedPlayer = joinedPlayers.Find((player) => player.playerInput == playerInput);
        joinedPlayer.spawnpoint.occupied = false;
        joinedPlayers.Remove(joinedPlayer);

        Debug.Log($"Lost Player {joinedPlayer.ID}: {joinedPlayer.gamepad.name}");

        this.CallNextFrame(Destroy, playerInput.gameObject);
    }

    private SpawnPoint GetNextFreeSpawnpoint()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            var spawnpoint = spawnPoints[i];
            if (!spawnpoint.occupied)
            {
                return spawnpoint;
            }
        }

        return spawnPoints[0];
    }

    private void OnLevelLoaded(Level level)
    {
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
    }

    private void OnAnyPlayerEnterFinishArea()
    {
        // Start game if any player enter the finish area
        var players = joinedPlayers.Select(player => player.playerInput.GetComponent<Player>());
        EventBus.Instance?.OnStartGame?.Invoke(players.ToArray());

        // Disable self because player spawning during game is not intended
        this.enabled = false;

        // Unhook connected events
        foreach (var joinedPlayer in joinedPlayers)
        {
            var player = joinedPlayer.playerInput.GetComponent<Player>();
            player.OnFinishedRound -= OnAnyPlayerEnterFinishArea;
        }
    }
}
