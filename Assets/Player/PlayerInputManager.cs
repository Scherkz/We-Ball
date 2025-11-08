using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPointsParents;

    private HashSet<Gamepad> joinedGamepads = new();
    private List<PlayerInput> players = new();

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

        player.transform.position = spawnPointsParents.GetChild(players.Count).position;
        player.GetComponent<Renderer>().material.color = GetRandomColor();

        players.Add(player);
    }

    private static Color GetRandomColor()
    {
        return Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
    }
}
