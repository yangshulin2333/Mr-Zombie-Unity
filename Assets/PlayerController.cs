using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // 📢 1. 必须引入 UI 命名空间，否则不认识 Slider

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("UI Settings")] // 在面板里加个标题，好看
    public Slider breathSlider; // 📢 2. 声明一个槽位，用来放那个 UI 条

    [Header("Breath Settings")]
    public float maxBreath = 100f;   // 最大肺活量
    public float breathRate = 20f;   // 消耗速度 (每秒减20，5秒用完)
    public float recoverRate = 10f;  // 恢复速度

    private float currentBreath;     // 当前剩余肺活量
    private bool isChoking = false;  // 是否呛到了 (冷却状态)

    private GameControls controls;
    private Vector2 moveInput;
    private bool isHoldingBreath;

    void Awake()
    {
        controls = new GameControls();
        controls.Gameplay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveInput = Vector2.zero;

        // 按下空格 -> 开始屏息
        controls.Gameplay.Breath.started += ctx => StartBreath();
        // 松开空格 -> 停止屏息
        controls.Gameplay.Breath.canceled += ctx => EndBreath();

        // 初始满血
        currentBreath = maxBreath;
    }

    void OnEnable() => controls.Gameplay.Enable();
    void OnDisable() => controls.Gameplay.Disable();

    void Update()
    {
        // --- 移动逻辑 ---
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // --- 📢 3. 屏息核心逻辑 (新增) ---
        HandleBreathLogic();
    }


    void HandleBreathLogic()
    {
        // --- 逻辑判断区 ---

        if (isChoking)
        {
            // 情况1: 正在咳嗽冷却中 -> 强制恢复
            RecoverBreath();
        }
        else if (isHoldingBreath) // 注意这里用了 else if (否则如果)
        {
            // 情况2: 没咳嗽，且按着空格 -> 扣血
            currentBreath -= breathRate * Time.deltaTime;

            if (currentBreath <= 0)
            {
                currentBreath = 0;
                isChoking = true;
                EndBreath();
                Debug.Log(">>> 咳咳咳！憋不住了！");
                Invoke("StopChoking", 3f);
            }
        }
        else
        {
            // 情况3: 既没咳嗽，也没按空格 -> 自然恢复
            RecoverBreath();
        }

        // --- 视觉表现区 ---

        // 📢 关键修复：现在这行代码在最外面
        // 无论上面走了哪个分支，最后都会执行这里！
        if (breathSlider != null)
        {
            breathSlider.value = currentBreath;
        }
    }

    void RecoverBreath()
    {
        currentBreath += recoverRate * Time.deltaTime;
        // Mathf.Clamp 限制数值，不能超过最大值
        currentBreath = Mathf.Clamp(currentBreath, 0, maxBreath);
    }

    void StopChoking()
    {
        isChoking = false;
        Debug.Log(">>> 呼吸平复了。");
    }

    void StartBreath()
    {
        // 如果在咳嗽，按空格也没用
        if (!isChoking)
        {
            isHoldingBreath = true;
        }
    }

    void EndBreath()
    {
        isHoldingBreath = false;
    }
}