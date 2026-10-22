using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI prepareText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChanged; //订阅游戏状态改变事件

        //初始化游戏状态
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged; //取消订阅游戏状态改变事件
        }
    }

    /// <summary>
    /// 游戏状态改变事件
    /// </summary>
    /// <param name="newState">新的游戏状态</param>
    void HandleStateChanged(GameState newState)
    {
        StopAllCoroutines();
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
                StartCoroutine(ShowPrepareText());
                break;
            case GameState.GameOver:
                StartCoroutine(ShowGameOverPanel());
                break;
        }
    }

    /// <summary>
    /// 显示游戏结束面板
    /// </summary>
    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(1.5f);
        gameOverPanel.SetActive(true);
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

    /// <summary>
    /// 显示准备文本
    /// </summary>
    IEnumerator ShowPrepareText()
    {
        prepareText.text = "3";
        yield return new WaitForSeconds(1f);
        prepareText.text = "2";
        yield return new WaitForSeconds(1f);
        prepareText.text = "1";
        yield return new WaitForSeconds(1f);
        prepareText.text = "Go";
        yield return new WaitForSeconds(0.5f);
    } 
}
