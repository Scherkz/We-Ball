using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverScreen : MonoBehaviour
{
    [SerializeField] private string[] taunts;
    [SerializeField] private InputAction rematchInputAction;

    private Image colorCircle;
    private TMP_Text winnerTaunt;

    private void Awake()
    {
        colorCircle = transform.Find("ColorCircle").GetComponent<Image>();
        winnerTaunt = transform.Find("WinnerTaunt").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        rematchInputAction.Disable();
        ToggleCildren(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnWinnerDicided += OnWinnerDicided;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnWinnerDicided -= OnWinnerDicided;
    }

    private void Update()
    {
        if (rematchInputAction.WasReleasedThisFrame())
        {
            OnRematch();
        }
    }

    private void OnWinnerDicided(Player player)
    {
        rematchInputAction.Enable();
        ToggleCildren(true);

        colorCircle.color = player.GetColor();
        winnerTaunt.text = taunts[Random.Range(0, taunts.Length)];
    }

    private void OnRematch()
    {
        // Reload scene to restart game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ToggleCildren(bool enable)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enable);
        }
    }
}
