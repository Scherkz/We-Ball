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

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player[]</term>
    ///         <description>Contains all the joined players.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player[]> OnStartGame;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>int</term>
    ///         <description>Contains the total amount of rounds to plays.</description>
    ///     </item>
    ///     <item>
    ///         <term>int</term>
    ///         <description>Contains the current amount of played rounds.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<int, int> OnRoundStart;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player</term>
    ///         <description>The winning player.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player> OnWinnerDicided;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player</term>
    ///         <description>The player that joined the game.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player> OnPlayerJoined;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player</term>
    ///         <description>The player that left the game.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player> OnPlayerLeft;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>MapNode</term>
    ///         <description>Contains the MapNode of the map the player voted for.</description>
    ///     </item>
    ///     <item>
    ///         <term>Player</term>
    ///         <description>The player who voted for the MapNode.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<MapNode, Player> OnMapVoted;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player</term>
    ///         <description>The player who toggled their ready button. This does NOT mean the player is ready!</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player> OnPlayerReady;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>MapNode</term>
    ///         <description>The map that was selected for playing after all players have voted and are ready. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<MapNode> OnMapSelected;

}