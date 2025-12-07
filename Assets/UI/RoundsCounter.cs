using TMPro;
using UnityEngine;

public class RoundsCounter : MonoBehaviour
{
    private TMP_Text text;

    public void ResetSelf()
    {
        text.gameObject.SetActive(false);
    }

    private void Awake()
    {
        text = transform.Find("RoundsText").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        ResetSelf();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnRoundStart += SetRoundsText;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnLevelLoaded -= OnLevelLoaded;
        EventBus.Instance.OnRoundStart -= SetRoundsText;
    }

    private void SetRoundsText(int maxRoundsCount, int currentRoundCount)
    {
        text.gameObject.SetActive(true);
        text.text = $"Round {currentRoundCount}/{maxRoundsCount}";
    }

    private void OnLevelLoaded(Level _level)
    {
        ResetSelf();
    }
}
