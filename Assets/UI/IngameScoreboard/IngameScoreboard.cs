using UnityEngine;

public class IngameScoreboard : MonoBehaviour
{
    [SerializeField] private GameObject playerSwingCounterPrefab;

    private void OnEnable()
    {
        EventBus.Instance.OnStartGame += OnStartGame;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnStartGame -= OnStartGame;
    }

    private void OnStartGame(Player[] players)
    {
        float prefabHeight = playerSwingCounterPrefab.GetComponent<RectTransform>().rect.height;
        float offset = 0;
        foreach (var player in players)
        {
            var counter = Instantiate(playerSwingCounterPrefab);
            counter.transform.SetParent(transform);
            counter.transform.localPosition = new Vector3(0, -offset, 0);

            var playerSwingCounter = counter.GetComponent<PlayerSwingCounter>();
            playerSwingCounter.SetPlayerColor(player.GetColor());
            playerSwingCounter.SetSwingsCounter(player.numberOfSwings);

            player.OnSwingsChanges += playerSwingCounter.SetSwingsCounter;

            offset += prefabHeight;
        }
    }
}
