using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip coinCollectSound; //金币收集音效
    public AudioClip dieSound; //死亡音效
    public AudioClip jumpSound; //跳跃音效
    public AudioClip slideSound; //滑行音效
    public AudioClip laneChangeSound; //车道切换音效
    public AudioClip buttonSound; //按钮音效

    private AudioSource audioSource;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        GameManager.Instance.OnStateChanged += PlayBGM;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= PlayBGM;
    }

    void PlayBGM(GameState newState)
    {
        if(newState == GameState.Playing)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void PlayCoinCollectSound()
    {
        audioSource.PlayOneShot(coinCollectSound);
    }

    public void PlayDieSound()
    {
        audioSource.PlayOneShot(dieSound);
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void PlaySlideSound()
    {
        audioSource.PlayOneShot(slideSound);
    }

    public void PlayLaneChangeSound()
    {
        audioSource.PlayOneShot(laneChangeSound);
    }

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
}
