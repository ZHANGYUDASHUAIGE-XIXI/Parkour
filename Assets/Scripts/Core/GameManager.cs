using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Menu;   //当前游戏状态

    public event Action<GameState> OnStateChanged;   //游戏状态改变事件

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("当前游戏状态：" + CurrentState);
    }

    /// <summary>
    /// 设置游戏状态
    /// </summary>
    /// <param name="newState"></param>
    void SetState(GameState newState)
    {
        if(CurrentState == newState) return;
        CurrentState = newState;
        Debug.Log("当前游戏状态：" + CurrentState);
        OnStateChanged?.Invoke(newState);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    public void PlayerDied()
    {
        if(CurrentState != GameState.Playing) return;
        SetState(GameState.GameOver);
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}