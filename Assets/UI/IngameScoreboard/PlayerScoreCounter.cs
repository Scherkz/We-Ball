using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreCounter : MonoBehaviour
{
    private Image colorImage;
    private TMP_Text scoreText;

    private void Awake()
    {
        colorImage = transform.Find("ColorCircle").GetComponent<Image>();
        scoreText = transform.Find("ScoreCount").GetComponent<TMP_Text>();
    }

    public void SetPlayerColor(Color color)
    {
        colorImage.color = color;
    }

    public void SetScoreCounter(int count)
    {
        scoreText.text = $"Score: {count}";
    }
}
