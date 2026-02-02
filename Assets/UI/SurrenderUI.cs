using TMPro;
using UnityEngine;

public class SurrenderUI : MonoBehaviour
{
    private TMP_Text text;

    private bool disabled;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        SetVisible(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnToggleSurrenderHint += SetVisible;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnLevelLoaded -= OnLevelLoaded;
        EventBus.Instance.OnToggleSurrenderHint -= SetVisible;
    }

    private void SetVisible(bool visible)
    {
        if (disabled)
            visible = false;

        text.enabled = visible;
    }

    private void OnLevelLoaded(Level _level, bool isLobby)
    {
        if (isLobby)
        {
            SetVisible(false);
        }

        disabled = isLobby;
    }
}
