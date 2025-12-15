using System.Collections.Generic;
using UnityEngine;

public class BlueShellPowerUp : PowerUpBuilding
{
    [SerializeField] private BlueShellMissile missilePrefab;
    
    [SerializeField] private PlayerRegistry playerRegistry;


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

            Transform ball = leader.GetBallTransform();
            if (ball == null) continue;

            Vector3 spawnPos = transform.position;
            var missile = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
            
            missile.Launch(ball, Random.Range(0f, 360f));
        }
    }
    
    private List<Player> FindPlayersInLead()
    {
        var bestScore = 0;
        var leaders = new List<Player>();
        foreach (var player in playerRegistry.players)
        {
            if (player.score > bestScore)
            {
                bestScore = player.score;
                leaders.Clear();
                leaders.Add(player);
            }
            else if (player.score == bestScore)
            {
                leaders.Add(player);
            }
        }

        return leaders;
    }
}
