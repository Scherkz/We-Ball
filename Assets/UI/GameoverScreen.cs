using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameoverScreen : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject circlePrefab;

    [SerializeField] private string[] taunts;
    [SerializeField] private InputAction rematchInputAction;

    private Image colorCircle;
    private TMP_Text winnerTaunt;
    private Transform scoreboard;

    private int cellHeight = 50;
    private int cellWidth = 50;

    private void Awake()
    {
        colorCircle = transform.Find("ColorCircle").GetComponent<Image>();
        winnerTaunt = transform.Find("WinnerTaunt").GetComponent<TMP_Text>();
        scoreboard = transform.Find("Scoreboard");
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

    private void OnWinnerDicided(Player winner, Player[] players, int roundsPlayed)
    {
        rematchInputAction.Enable();
        ToggleCildren(true);

        int rowsNeeded = players.Length + 1;
        int columnsNeeded = roundsPlayed + 2;
        GenerateScoreboard(rowsNeeded, columnsNeeded, players);
        colorCircle.color = winner.GetColor();
        winnerTaunt.text = taunts[Random.Range(0, taunts.Length)];
    }

    private void OnRematch()
    {
        // Reload scene to restart game
        int currentLevelBuildIndex = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).buildIndex;
        EventBus.Instance?.OnSwitchToScene?.Invoke(currentLevelBuildIndex);
    }

    private void ToggleCildren(bool enable)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enable);
        }
    }

    private void GenerateScoreboard(int totalRows, int totalColumns, Player[] players)
    {
        RectTransform rt = scoreboard.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(totalColumns * cellWidth, totalRows * cellHeight);

        for (int r = 0; r < totalRows; r++)
        {
            for (int c = 0; c < totalColumns; c++)
            {
                var cell = Instantiate(cellPrefab, scoreboard);
                var text = cell.GetComponent<TextMeshProUGUI>();

                bool isFirstRow = r == 0;
                bool isFirstColumn = c == 0;
                bool isLastColumn = c == totalColumns - 1;

                if (isFirstRow && isFirstColumn)
                {
                    text.text = "";
                }
                else if (isFirstRow && isLastColumn)
                {
                    text.text = "Total";
                }
                else if (isFirstRow)
                {
                    text.text = c.ToString();
                }
                else if (isFirstColumn && !isFirstRow)
                {
                    Destroy(cell);
                    var circleBall = Instantiate(circlePrefab, scoreboard);
                    var circleImage = circleBall.GetComponent<Image>();
                    circleImage.color = players[r - 1].GetColor();
                    RectTransform crt = circleBall.GetComponent<RectTransform>();
                    crt.sizeDelta = new Vector2(cellWidth, cellHeight);
                }
                else if (isLastColumn && !isFirstRow)
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
