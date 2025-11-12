using TMPro;
using UnityEngine;

public class RoundsCounter : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = transform.Find("RoundsText").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        text.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnRoundStart += SetRoundsText;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnRoundStart -= SetRoundsText;
    }

    private void SetRoundsText(int maxRoundsCount, int currentRoundCount)
    {
        text.gameObject.SetActive(true);
        text.text = $"Round {currentRoundCount}/{maxRoundsCount}";
    }
}
