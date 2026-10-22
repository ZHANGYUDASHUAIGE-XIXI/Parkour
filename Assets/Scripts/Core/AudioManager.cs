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
    public AudioSource sfxSource;    //音效源

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
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= PlayBGM;
        }
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
        sfxSource.PlayOneShot(coinCollectSound);
    }

    public void PlayDieSound()
    {
        sfxSource.PlayOneShot(dieSound);
    }

    public void PlayJumpSound()
    {
        sfxSource.PlayOneShot(jumpSound);
    }

    public void PlaySlideSound()
    {
        sfxSource.PlayOneShot(slideSound);
    }

    public void PlayLaneChangeSound()
    {
        sfxSource.PlayOneShot(laneChangeSound);
    }

    public void PlayButtonSound()
    {
        sfxSource.PlayOneShot(buttonSound);
    }
}
