using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRegistry", menuName = "Scriptable Objects/PlayerRegistry")]
public class PlayerRegistry : ScriptableObject
{
    public List<Player> players;

    private void OnValidate()
    {
        // Reset scriptable object for play mode
        players.Clear();
    }
}
