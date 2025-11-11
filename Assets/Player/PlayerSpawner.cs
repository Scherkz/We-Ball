using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    private struct SpawnPoint
    {
        public Vector3 position;
        public bool occupied;
    }

    private struct JoinedPlayer
    {
        public SpawnPoint spawnpoint;
        public Gamepad gamepad;
        public PlayerInput playerInput;
    }

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPointsParents;

    private readonly List<JoinedPlayer> joinedPlayers = new();
    private SpawnPoint[] spawnPoints;

    private int playerLastID = 0;

    private void Awake()
    {
        spawnPoints = new SpawnPoint[spawnPointsParents.childCount];
        for (int i = 0; i < spawnPointsParents.childCount; i++)
        {
            spawnPoints[i] = new SpawnPoint()
            {
                position = spawnPointsParents.GetChild(i).position,
                occupied = false
            };
        }
    }

    private void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            // gamepad already connected to player
            if (joinedPlayers.Any((player) => player.gamepad == gamepad))
            {
                continue;
            }

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                SpawnPlayer(gamepad);
            }
        }
    }

    private void SpawnPlayer(Gamepad gamepad)
    {
        Debug.Log($"Player {playerLastID} joining: {gamepad.device.name}");

        var playerInput = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamepad
        );

        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        playerInput.deviceLostEvent.AddListener(RemovePlayer);

        var spawnpoint = GetNextFreeSpawnpoint();
        spawnpoint.occupied = true;

        var player = playerInput.GetComponent<Player>();
        player.OnFinishedRound += OnAnyPlayerEnterFinishArea;
        player.StartPlayingPhase(spawnpoint.position);

        playerInput.gameObject.name = $"Player {playerLastID++} [{gamepad.device.displayName}]";

        joinedPlayers.Add(new JoinedPlayer()
        {
            spawnpoint = spawnpoint,
            gamepad = gamepad,
            playerInput = playerInput
        });
    }

    private void RemovePlayer(PlayerInput playerInput)
    {
        Debug.Log("Lost Player: " + playerInput.devices[0].name);

        var player = joinedPlayers.Find((player) => player.playerInput = playerInput);
        joinedPlayers.Remove(player);

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

    private void OnAnyPlayerEnterFinishArea()
    {
        // Start game if any player enter the finish area
        var players = joinedPlayers.Select(player => player.playerInput.GetComponent<Player>());
        EventBus.Instance?.OnStartGame?.Invoke(players.ToArray());
    }
}
