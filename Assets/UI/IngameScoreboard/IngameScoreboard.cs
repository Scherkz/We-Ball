using UnityEngine;

public class IngameScoreboard : MonoBehaviour
{
    [SerializeField] private GameObject playerScoreCounterPrefab;

    public void ResetSelf()
    {
        // clear all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnStartGame += OnStartGame;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnLevelLoaded -= OnLevelLoaded;
        EventBus.Instance.OnStartGame -= OnStartGame;
    }

    private void OnStartGame(Player[] players)
    {
        // spawn a PlayerScoreCounter for each player
        float prefabHeight = playerScoreCounterPrefab.GetComponent<RectTransform>().rect.height;
        float offset = 0;
        foreach (var player in players)
        {
            var counter = Instantiate(playerScoreCounterPrefab);
            counter.transform.SetParent(transform);
            counter.transform.localPosition = new Vector3(0, -offset, 0);

            var playerScoreCounter = counter.GetComponent<PlayerScoreCounter>();
            playerScoreCounter.SetPlayerColor(player.GetColor());
            playerScoreCounter.SetScoreCounter(0);

            player.OnScoreChanges += playerScoreCounter.SetScoreCounter;
            player.OnSpecialShotAssigned += playerScoreCounter.SetSpecialShotType;

            offset += prefabHeight;
        }
    }

    private void OnLevelLoaded(Level _level)
    {
        ResetSelf();
    }
}
