using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueShellPowerUp : PowerUpBuilding
{
    
    [SerializeField] private BlueShellMissile missilePrefab;
    
    [SerializeField] private Vector3 missileSpawnOffset = new Vector3(0f, 0.1f, 0f);

    private Player[] players = { };

    protected override void OnCollected(Player collectingPlayer, PlayerController collectingController)
    {
        if (missilePrefab == null)
            return;
        var leaders = FindPlayersInLead();
        if (leaders == null || leaders.Count == 0)
            return;
        
        foreach (var leader in leaders)
        {
            if (leader == null) continue;

            Transform ball = leader.transform.Find("PlayerBall");
            if (ball == null) continue;

            Vector3 spawnPos = transform.position + missileSpawnOffset;
            var missile = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

            missile.Launch(ball, Random.Range(0f, 360f));
        }
    }
    
    private List<Player> FindPlayersInLead()
    {
        players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        
        
        players = players.OrderBy(player => player.score).ToArray();
        
        var bestScore = players.Max(p => p.score);
        return players.Where(p => p.score == bestScore).ToList();
    }
}
