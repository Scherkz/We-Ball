using UnityEngine;

public class SplitBallLifetime : MonoBehaviour
{
    private Player player;
    private GameObject cloneBall;

    private bool cleanupPending;

    private GameObject playerBuildingGO;

    public void Init(Player player, GameObject cloneBall)
    {
        this.player = player;
        this.cloneBall = cloneBall;

        var playerBuilding = player.transform.Find("PlayerBuilding");
        playerBuildingGO = playerBuilding != null ? playerBuilding.gameObject : null;
        
        player.OnFinishedRound += OnFinishedRound;
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnFinishedRound -= OnFinishedRound;
    }

    private void OnFinishedRound()
    {
        cleanupPending = true;
    }

    private void Update()
    {
        if (!cleanupPending)
            return;

        if (playerBuildingGO != null && playerBuildingGO.activeSelf)
        {
            if (cloneBall != null)
                Destroy(cloneBall);

            Destroy(gameObject); 
        }
    }
}
