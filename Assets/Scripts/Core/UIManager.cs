using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChanged; //订阅游戏状态改变事件

        //初始化游戏状态
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= HandleStateChanged; //取消订阅游戏状态改变事件
    }

    /// <summary>
    /// 游戏状态改变事件
    /// </summary>
    /// <param name="newState">新的游戏状态</param>
    void HandleStateChanged(GameState newState)
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        
        switch(newState)
        {
            case GameState.Menu:
                menuPanel.SetActive(true);
                break;
            case GameState.Playing:
                hudPanel.SetActive(true);
                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 开始游戏点击事件
    /// </summary>
    public void OnStartGameClick()
    {
        GameManager.Instance.StartGame();
        //播放开始游戏音效
        AudioManager.Instance.PlayButtonSound();
    }

    /// <summary>
    /// 重新开始游戏点击事件
    /// </summary>
    public void OnRestartGameClick()
    {
        GameManager.Instance.RestartGame();
        //播放重新开始游戏音效
        AudioManager.Instance.PlayButtonSound();
    }
}
