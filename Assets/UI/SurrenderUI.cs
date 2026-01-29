using TMPro;
using UnityEngine;

public class SurrenderUI : MonoBehaviour
{
    private TMP_Text text;
    
    private bool isLobby;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        SetVisible(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.InLobby += OnLobbyChanged;
        EventBus.Instance.OnToggleSurrenderHint += SetVisible;
    }

    private void OnDisable()
    {
        EventBus.Instance.InLobby -= OnLobbyChanged;
        EventBus.Instance.OnToggleSurrenderHint -= SetVisible;
    }

    private void SetVisible(bool visible)
    {
        if (isLobby)
        {
            visible = false;
        }
        
        text.enabled = visible;
    }
    private void OnLobbyChanged(bool lobby)
    {
        isLobby = lobby;

        if (isLobby)
        {
            text.enabled = false;
        }
    }
}
