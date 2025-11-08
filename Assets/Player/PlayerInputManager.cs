using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
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

    private readonly List<JoinedPlayer> players = new();
    private SpawnPoint[] spawnPoints;

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
            if (players.Any((player) => player.gamepad == gamepad))
                continue;

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                SpawnPlayer(gamepad);
            }
        }
    }

    private void SpawnPlayer(Gamepad gamepad)
    {
        Debug.Log("Player joined: " + gamepad.device.name);

        var playerInput = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamepad
        );

        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        playerInput.deviceLostEvent.AddListener(RemovePlayer);

        var spawnpoint = GetNextFreeSpawnpoint();
        spawnpoint.occupied = true;

        playerInput.transform.position = spawnpoint.position;
        playerInput.GetComponent<Renderer>().material.color = GetPlayerColor();

        players.Add(new JoinedPlayer()
        {
            spawnpoint = spawnpoint,
            gamepad = gamepad,
            playerInput = playerInput
        });
    }

    private void RemovePlayer(PlayerInput playerInput)
    {
        Debug.Log("Lost Player: " + playerInput.devices[0].name);


        var player = players.Find((player) => player.playerInput = playerInput);
        players.Remove(player);

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

    private Color GetPlayerColor()
    {
        float maxPlayerCount = spawnPointsParents.childCount;
        return Color.HSVToRGB(players.Count / maxPlayerCount, 1f, 1f);
    }
}
