using UnityEngine;
using TMPro;

public class SurrenderUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        Debug.Log("SurrenderHintUI Awake on: " + gameObject.name);
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        SetVisible(false); 
    }

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnSurrenderHintVisible += SetVisible;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnSurrenderHintVisible -= SetVisible;
        }
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null) 
            return;
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
