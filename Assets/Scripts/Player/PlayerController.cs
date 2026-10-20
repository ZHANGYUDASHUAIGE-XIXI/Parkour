using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTrans;   //摄像机
    private float offsetZ;          //相机偏移量

    [SerializeField]
    private float speed = 10f;      //角色移动速度
    private float maxSpeed = 16f;   //最大移动速度
    private float speedUpAmount = 0.4f; //速度增加量
    private float nextSpeedUpZ = 100f; //下一个速度增加位置

    private int laneIndex = 0;              //-1左 0中 1右
    private float laneWidth = 4f;           //道路宽度
    private float laneChangeSpeed = 30f;    //变道速度

    private float jumpSpeed = 15f;  //跳跃速度
    private float gravity = -40f;   //重力加速度
    private float velocityY = 0f;   //当前起跳速度

    private CharacterController controller; //角色控制器

    private bool isSliding = false;     //是否正在滑铲
    private float slideDuration = 0.8f; //滑铲持续时间
    private float slideTimer = 0f;      //滑铲计时器
    private float standHeight = 2f;     //站立高度
    private float slideHeight = 1f;     //滑铲高度
    private Vector3 standCenter;        //站立时碰撞体中心

    private Animator animator;          //动画控制器

    // Start is called before the first frame update
    void Start()
    { 
        //相机偏移量
        offsetZ = cameraTrans.position.z - transform.position.z;
        //胶囊碰撞体
        controller = GetComponent<CharacterController>();
        //站立时碰撞体中心
        standCenter = controller.center;
        //动画控制器
        animator = GetComponent<Animator>();

        GameManager.Instance.OnStateChanged += PlayRunAnimation;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= PlayRunAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        if(speed < maxSpeed && transform.position.z >= nextSpeedUpZ)
        {
            speed += speedUpAmount;
            speed = Mathf.Clamp(speed, 0f, maxSpeed);
            nextSpeedUpZ += 100f;
        }

        Vector3 moveDelta = Vector3.zero;
        moveDelta.x = GetLaneDelta();
        moveDelta.y = GetJumpDelta();
        moveDelta.z = GetForwardDelta();

        controller.Move(moveDelta);

        PlayerSlide();
    }

    private void LateUpdate()
    {
        CameraFollow();
    }

    /// <summary>
    /// 获取向前位移
    /// </summary>
    /// <returns></returns>
    float GetForwardDelta()
    {
        return speed * Time.deltaTime;
    }

    /// <summary>
    /// 相机跟随角色
    /// </summary>
    void CameraFollow()
    {
        Vector3 pos = cameraTrans.position;
        pos.z = transform.position.z + offsetZ;
        cameraTrans.position = pos;
    }

    /// <summary>
    /// 角色变道输入
    /// </summary>
    void PlayerLaneInput()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            laneIndex--;
            if (laneIndex < -1) laneIndex = -1;
            //播放变道音效
            AudioManager.Instance.PlayLaneChangeSound();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            laneIndex++;
            if (laneIndex > 1) laneIndex = 1;
            //播放变道音效
            AudioManager.Instance.PlayLaneChangeSound();
        }
    }

    /// <summary>
    /// 获取变道位移
    /// </summary>
    /// <returns></returns>
    float GetLaneDelta()
    {
        PlayerLaneInput();

        float targetX = laneIndex * laneWidth;
        float currentX = transform.position.x;
        float newX = Mathf.MoveTowards(currentX, targetX, laneChangeSpeed * Time.deltaTime);

        return newX - currentX;
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    public void Die()
    {
        GameManager.Instance.PlayerDied();
        AudioManager.Instance.PlayDieSound();
        animator.SetBool("IsDead", true);
    }

    /// <summary>
    /// 获取跳跃位移
    /// </summary>
    /// <returns></returns>
    float GetJumpDelta()
    {
        if (controller.isGrounded)
        {
            if(velocityY < 0f) velocityY = -2f;
            //播放奔跑动画
            animator.SetBool("IsJumping", false);
        }

        if(Input.GetKeyDown(KeyCode.Space) && controller.isGrounded && !isSliding)
        {
            velocityY = jumpSpeed;
            //播放跳跃动画
            animator.SetBool("IsJumping", true);
            //播放跳跃音效
            AudioManager.Instance.PlayJumpSound();
        }

        velocityY += gravity * Time.deltaTime;

        return velocityY * Time.deltaTime;
    }

    /// <summary>
    /// 角色滑铲
    /// </summary>
    void PlayerSlide()
    {
        if(Input.GetKeyDown(KeyCode.S) && !isSliding && controller.isGrounded)
        {
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if(slideTimer <= 0)
            {
                EndSlide();
            }
        }
    }

    /// <summary>
    /// 开始滑铲
    /// </summary>
    void StartSlide()
    {
        //播放滑铲动画
        animator.SetBool("IsSliding", true);
        //播放滑铲音效
        AudioManager.Instance.PlaySlideSound();

        isSliding = true;
        slideTimer = slideDuration;

        controller.height = slideHeight;
        float bottom = standCenter.y - standHeight * 0.5f;
        controller.center = new Vector3(standCenter.x, bottom + slideHeight * 0.5f, standCenter.z);
    }

    /// <summary>
    /// 结束滑铲
    /// </summary>
    void EndSlide()
    {
        //播放奔跑动画
        animator.SetBool("IsSliding", false);

        isSliding = false;
        slideTimer = 0f;

        controller.height = standHeight;
        controller.center = standCenter;
    }

    void PlayRunAnimation(GameState newState)
    {
        if(newState == GameState.Playing)
        {
            animator.SetBool("IsPlaying", true);
        }
        else if(newState == GameState.Menu)
        {
            animator.SetBool("IsPlaying", false);
        }
    }
}
