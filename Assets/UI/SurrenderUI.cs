using TMPro;
using UnityEngine;

public class SurrenderUI : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        SetVisible(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnToggleSurrenderHint += SetVisible;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnToggleSurrenderHint -= SetVisible;
    }

    private void SetVisible(bool visible)
    {
        text.enabled = visible;
    }
}
