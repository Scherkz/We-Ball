using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoadingScreen : MonoBehaviour
{
    private Image mapIcon;
    private TMP_Text selectionText;
    private TMP_Text countdownText;

    private Coroutine countdownCoroutine;

    private void Awake()
    {
        mapIcon = transform.Find("MapIcon").GetComponent<Image>();
        selectionText = transform.Find("SelectionText").GetComponent<TMP_Text>();
        countdownText = transform.Find("CountdownText").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        ToggleCildren(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnMapSelected += ShowLoadingScreen;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnMapSelected -= ShowLoadingScreen;
    }

    private void ShowLoadingScreen(MapNode map, int countdown)
    {
        ToggleCildren(true);

        selectionText.text = $"{map.mapName} has been selected!";
        mapIcon.sprite = map.mapIcon;

        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        countdownCoroutine = StartCoroutine(CountdownCoroutine(countdown));
    }

    private IEnumerator CountdownCoroutine(int countdown)
    {
        while (countdown > 0)
        {
            countdownText.text = $"Starting game in {countdown}...";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = $"Starting game in {countdown}...";
    }

    private void ToggleCildren(bool enable)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enable);
        }
    }

}
