using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTrans;   //摄像机
    private float offsetZ;          //相机偏移量

    private float speed = 10f;  //角色移动速度

    private int laneIndex = 0;              //-1左 0中 1右
    private float laneWidth = 4f;           //道路宽度
    private float laneChangeSpeed = 30f;    //变道速度

    private bool isDead = false;    //是否死亡

    private float jumpSpeed = 15f;  //跳跃速度
    private float gravity = -40f;   //重力加速度
    private float velocityY = 0f;   //当前起跳速度
    private float groundY;          //地面高度

    private CharacterController controller; //角色控制器

    private bool isSliding = false;     //是否正在滑铲
    private float slideDuration = 0.8f; //滑铲持续时间
    private float slideTimer = 0f;      //滑铲计时器
    private float standHeight = 2f;     //站立高度
    private float slideHeight = 1f;     //滑铲高度
    private Vector3 standCenter;        //站立时碰撞体中心

    // Start is called before the first frame update
    void Start()
    { 
        //相机偏移量
        offsetZ = cameraTrans.position.z - transform.position.z;
        //地面高度
        groundY = transform.position.y;
        //胶囊碰撞体
        controller = GetComponent<CharacterController>();
        //站立时碰撞体中心
        standCenter = controller.center;
    }

    // Update is called once per frame
    void Update()
    {
        //按R重新加载游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (isDead) return;

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
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            laneIndex++;
            if (laneIndex > 1) laneIndex = 1;
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
        isDead = true;
        Debug.Log("角色死亡！按R重新开始游戏！");
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
        }

        if(Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocityY = jumpSpeed;
        }

        velocityY += gravity * Time.deltaTime;

        return velocityY * Time.deltaTime;
    }

    /// <summary>
    /// 角色滑铲
    /// </summary>
    void PlayerSlide()
    {
        if(Input.GetKeyDown(KeyCode.S) && !isSliding)
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
        isSliding = false;
        slideTimer = 0f;

        controller.height = standHeight;
        controller.center = Vector3.zero;
    }
}
