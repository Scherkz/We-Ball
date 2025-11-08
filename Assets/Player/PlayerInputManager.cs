using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPointsParents;

    private readonly HashSet<Gamepad> joinedGamepads = new();
    private readonly List<PlayerInput> players = new();

    private void Update()
    {
        foreach (var gamepad in Gamepad.all)
        {
            if (joinedGamepads.Contains(gamepad))
                continue;

            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                joinedGamepads.Add(gamepad);

                SpawnPlayer(gamepad);
            }
        }
    }

    private void SpawnPlayer(Gamepad gamepad)
    {
        var player = PlayerInput.Instantiate(
            playerPrefab,
            controlScheme: "Gamepad",
            pairWithDevice: gamepad
        );

        player.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        player.deviceLostEvent.AddListener(RemovePlayer);

        player.transform.position = spawnPointsParents.GetChild(players.Count).position;
        player.GetComponent<Renderer>().material.color = GetPlayerColor();

        players.Add(player);
    }

    private void RemovePlayer(PlayerInput player)
    {
        Debug.Log("Lost Player: " + player.devices[0].name);

        players.Remove(player);

        this.CallNextFrame(Destroy, player.gameObject);

        foreach (var device in player.devices)
        {
            joinedGamepads.RemoveWhere((gamepad) => gamepad.device == device);
        }
    }

    private Color GetPlayerColor()
    {
        float maxPlayerCount = spawnPointsParents.childCount;
        return Color.HSVToRGB(players.Count / maxPlayerCount, 1f, 1f);
    }
}
