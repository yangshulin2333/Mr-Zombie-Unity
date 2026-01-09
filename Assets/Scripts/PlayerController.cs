using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("UI Settings")]
    public Slider breathSlider;

    [Header("Breath Settings")]
    public float maxBreath = 100f;
    public float breathRate = 20f;
    public float recoverRate = 10f;

    // 给僵尸看的接口
    public bool IsHidden => isHoldingBreath;

    private float currentBreath;
    private bool isChoking = false;

    private GameControls controls;
    private Vector2 moveInput;
    private bool isHoldingBreath;

    // 👇 1. 新增：我们要控制那个子物体身上的 Animator
    private Animator anim;

    void Awake()
    {
        controls = new GameControls();
        // 绑定输入事件
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Gameplay.Breath.started += ctx => StartBreath();
        controls.Gameplay.Breath.canceled += ctx => EndBreath();

        currentBreath = maxBreath;

        // 👇 2. 关键连接：去子物体(Survivor)身上找 Animator 组件
        // 因为脚本在父物体Player上，Animator在子物体Survivor上，所以必须用 GetComponentInChildren
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Update()
    {

        // 如果正在屏息(isHoldingBreath)，强制把移动输入(moveInput)归零
        if (isHoldingBreath)
        {
            moveInput = Vector2.zero;
        }
        // --- 移动逻辑 ---
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // 👇 3. 面朝向优化：让模型看着移动方向 (不然它会侧着身子跑)
        if (moveInput != Vector2.zero)
        {
            // 让子物体(模型)转头，而不是父物体转，这样不会打乱坐标系
            if (anim != null)
            {
                anim.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }

        // --- 👇 4. 动画驱动核心 ---
        if (anim != null)
        {
            // 有没有在移动？(向量长度大于0就是动了)
            bool isRun = moveInput.sqrMagnitude > 0.1f;

            // 把状态传给 Animator (对应你刚才画的那些线和参数)
            anim.SetBool("isMoving", isRun);
            anim.SetBool("isHidden", isHoldingBreath);
        }

        HandleBreathLogic();
    }

    // --- 下面是原来的屏息逻辑，不用动 ---
    void HandleBreathLogic()
    {
        if (isChoking)
        {
            RecoverBreath();
        }
        else if (isHoldingBreath)
        {
            currentBreath -= breathRate * Time.deltaTime;
            if (currentBreath <= 0)
            {
                currentBreath = 0;
                isChoking = true;
                EndBreath();
                // Debug.Log("咳咳咳！"); // 可以把这行注释掉
                Invoke("StopChoking", 3f);
            }
        }
        else
        {
            RecoverBreath();
        }

        if (breathSlider != null) breathSlider.value = currentBreath;
    }

    void RecoverBreath()
    {
        currentBreath += recoverRate * Time.deltaTime;
        currentBreath = Mathf.Clamp(currentBreath, 0, maxBreath);
    }

    void StopChoking()
    {
        isChoking = false;
    }

    void StartBreath()
    {
        if (!isChoking) isHoldingBreath = true;
    }

    void EndBreath()
    {
        isHoldingBreath = false;
    }
}