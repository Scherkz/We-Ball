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
    public Action<Player[]> OnStartGame;

    /// <summary>
    /// <list type="number">
    ///     <item>
    ///         <term>Level</term>
    ///         <description>Contains the currently loaded level.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public Action<Level> OnLevelLoaded;

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
    public Action<Player, Player[], int> OnWinnerDicided;
}