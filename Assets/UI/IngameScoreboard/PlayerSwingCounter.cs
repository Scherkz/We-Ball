using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSwingCounter : MonoBehaviour
{
    private Image colorImage;
    private TMP_Text swingText;

    private void Awake()
    {
        colorImage = transform.Find("ColorCircle").GetComponent<Image>();
        swingText = transform.Find("SwingsCount").GetComponent<TMP_Text>();
    }

    public void SetPlayerColor(Color color)
    {
        colorImage.color = color;
    }

    public void SetSwingsCounter(int count)
    {
        swingText.text = $"Swings: {count}";
    }
}
