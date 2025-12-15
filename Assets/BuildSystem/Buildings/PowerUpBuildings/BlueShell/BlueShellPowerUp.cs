using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlueShellPowerUp : PowerUpBuilding
{
    [SerializeField] private BlueShellMissile missilePrefab;
    
    [SerializeField] private Vector3 missileSpawnOffset = new Vector3(0f, 0.1f, 0f);

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

            Vector3 spawnPos = transform.position + missileSpawnOffset;
            var missile = Instantiate(missilePrefab, spawnPos, Quaternion.identity);

            missile.Launch(ball, Random.Range(0f, 360f));
        }
    }
    
    private List<Player> FindPlayersInLead()
    {
        var bestScore = int.MinValue;
        
        foreach (var pi in PlayerInput.all)
        {
            var p = pi.GetComponent<Player>();
            if (p == null) 
                continue;
            if (p.score > bestScore)
            {
                bestScore = p.score;
            }
        }
        
        var leaders = new List<Player>();
        foreach (var pi in PlayerInput.all)
        {
            var p = pi.GetComponent<Player>();
            if (p == null) 
                continue;
            if (p.score == bestScore)
            {
                leaders.Add(p);
            }
        }
        
        return leaders;
    }
}
