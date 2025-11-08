using System;
using UnityEngine;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            string parentName = transform.parent != null ? transform.parent.name + "/" : "";
            Debug.LogWarning("Multiple Instances of EventBus found! Destroying this: " + parentName + transform.name);
            Destroy(gameObject);
        }
    }

    public Action<PlayerController[]> OnStartGame;

    public Action<PlayerInfo> OnPlayerPlacedBuilding;
    public Action<PlayerInfo> OnPlayerFinishedRound;
}