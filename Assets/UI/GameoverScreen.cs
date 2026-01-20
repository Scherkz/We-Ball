using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameoverScreen : MonoBehaviour
{
    private const int LOBBY_SCENE_BUILD_INDEX = 1;

    public GameObject cellPrefab;
    public GameObject circlePrefab;

    [SerializeField] private string[] taunts;
    [SerializeField] private InputAction rematchInputAction;

    [SerializeField] private AudioSource gameoverSfx;

    private Image colorCircle;
    private TMP_Text winnerTaunt;
    private Transform scoreboard;

    private int cellHeight = 50;
    private int cellWidth = 50;

    public void ResetSelf()
    {
        rematchInputAction.Disable();
        ToggleCildren(false);

        for (int i = scoreboard.childCount - 1; i >= 0; i--)
        {
            Destroy(scoreboard.GetChild(i).gameObject);
        }
    }

    private void Awake()
    {
        colorCircle = transform.Find("ColorCircle").GetComponent<Image>();
        winnerTaunt = transform.Find("WinnerTaunt").GetComponent<TMP_Text>();
        scoreboard = transform.Find("Scoreboard");
    }

    private void Start()
    {
        ResetSelf();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnWinnerDecided += OnWinnerDecided;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnLevelLoaded += OnLevelLoaded;
        EventBus.Instance.OnWinnerDecided -= OnWinnerDecided;
    }

    private void Update()
    {
        if (rematchInputAction.WasReleasedThisFrame())
        {
            OnRematch();
        }
    }

    private void OnWinnerDecided(Player winner, Player[] players, int roundsPlayed)
    {
        if (gameoverSfx != null)
            gameoverSfx.Play();

        rematchInputAction.Enable();
        ToggleCildren(true);

        GenerateScoreboard(players, roundsPlayed);
        colorCircle.color = winner.GetColor();
        winnerTaunt.text = taunts[Random.Range(0, taunts.Length)];
    }

    private void OnRematch()
    {
        // Switch to lobby scene after game is over
        EventBus.Instance?.OnSwitchToScene?.Invoke(LOBBY_SCENE_BUILD_INDEX);
    }

    private void ToggleCildren(bool enable)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enable);
        }
    }

    private void GenerateScoreboard(Player[] players, int roundsPlayed)
    {
        int totalRows = players.Length + 1;
        int totalColumns = roundsPlayed + 2;

        RectTransform rt = scoreboard.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(totalColumns * cellWidth, totalRows * cellHeight);

        for (int r = 0; r < totalRows; r++)
        {
            for (int c = 0; c < totalColumns; c++)
            {
                bool isFirstRow = r == 0;
                bool isFirstColumn = c == 0;
                bool isLastColumn = c == totalColumns - 1;

                if (isFirstColumn && !isFirstRow)
                {
                    var circleBall = Instantiate(circlePrefab, scoreboard);

                    var circleImage = circleBall.GetComponent<Image>();
                    circleImage.color = players[r - 1].GetColor();

                    RectTransform crt = circleBall.GetComponent<RectTransform>();
                    crt.sizeDelta = new Vector2(cellWidth, cellHeight);
                    continue;
                }

                var cell = Instantiate(cellPrefab, scoreboard);
                var text = cell.GetComponent<TextMeshProUGUI>();

                if (isFirstRow)
                {
                    if (isFirstColumn)
                    {
                        text.text = "";
                    }
                    else if (isLastColumn)
                    {
                        text.text = "Total";
                    }
                    else
                    {
                        text.text = c.ToString();
                    }
                }
                else
                {
                    if (isLastColumn)
                    {
                        text.text = players[r - 1].score.ToString();
                    }
                    else
                    {
                        text.text = players[r - 1].scorePerRound[c - 1].ToString();
                    }
                }
            }
        }
    }

    private void OnLevelLoaded(Level _level, bool _isLobby)
    {
        ResetSelf();
    }
}
