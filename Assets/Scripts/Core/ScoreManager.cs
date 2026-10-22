using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public PlayerController player;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI currentScoreText;


    public int CoinCount { get; private set; } = 0;

    private int score = 0;
    private int lastScore = -1;
    private int lastCoin = -1;

    private const string HighScoreKey = "HighScore";

    // Start is called before the first frame update
    void Start()
    {
        Coin.OnCoinCollected += AddCoin;
        GameManager.Instance.OnStateChanged += ShowHighAndCurrentScore;

        RefreshHud();
    }

    void OnDestroy()
    {
        Coin.OnCoinCollected -= AddCoin;
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= ShowHighAndCurrentScore;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.CurrentState != GameState.Playing) return;
        score = Mathf.FloorToInt(player.transform.position.z) + CoinCount * 10;
        RefreshHud();
    }

    void RefreshHud()
    {
        if(scoreText != null && score != lastScore)
        {
            lastScore = score;
            scoreText.text = "得分：" + score;
        }
        if(coinText != null && CoinCount != lastCoin)
        {
            lastCoin = CoinCount;
            coinText.text = "金币：" + CoinCount;
        }
    }

    void AddCoin(int count)
    {
        if(GameManager.Instance.CurrentState != GameState.Playing) return;
        CoinCount += count;
        RefreshHud();
    }

    int UpdateAndGetHighScore()
    {
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        return highScore;
    }

    void ShowHighAndCurrentScore(GameState newState)
    {
        if(newState != GameState.GameOver) return;
        int highScore = UpdateAndGetHighScore();
        if(highScoreText != null) highScoreText.text = "最高得分：" + highScore;
        if(currentScoreText != null) currentScoreText.text = "当前得分：" + score;
    }
}
