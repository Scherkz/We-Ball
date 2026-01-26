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
    ///         <term>int</term>
    ///         <description>Contains the buildIndex of the scene that should be switched to.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<int> OnSwitchToScene;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Player[]</term>
    ///         <description>Contains all the joined players.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player[]> OnAnnouncePlayers;

    /// <summary>
    /// </summary>
    public Action OnStartGame;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Level</term>
    ///         <description>Contains the currently loaded level.</description>
    ///     </item>
    ///     <item>
    ///         <term>bool</term>
    ///         <description>Is the level the lobby.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Level, bool> OnLevelLoaded;

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
    ///     <item>
    ///         <term>Player[]</term>
    ///         <description>Contains all the joined players.</description>
    ///     </item>
    ///     <item>
    ///         <term>int</term>
    ///         <description>Contains the total amount of rounds played</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Player, Player[] ,int> OnWinnerDecided;

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
    ///         <term>MapNode</term>
    ///         <description>The map that was selected for playing after all players have voted. </description>
    ///     </item>
    ///     <item>
    ///         <term>int</term>
    ///         <description>The countdown time before the map is started. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<MapNode, int> OnMapSelected;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>bool</term>
    ///         <description>Should the hint be visible. </description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<bool> OnToggleSurrenderHint;

}