using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("僵尸设置")]
    public float chaseSpeed = 3.5f;

    private NavMeshAgent agent;
    private Transform player;
    private PlayerController playerScript;

    // 👇 1. 新增：我们要控制的动画组件
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        // 👇 2. 新增：自动去子物体里找 Animator
        // 因为脚本挂在父物体上，而 Animator 在子物体(那个模型)上
        // 所以必须用 GetComponentInChildren (注意有个 Children)
        anim = GetComponentInChildren<Animator>();

        var p = FindAnyObjectByType<PlayerController>();
        if (p != null)
        {
            player = p.transform;
            playerScript = p;
        }
    }

    void Update()
    {
        if (player == null) return;

        // --- 👇 3. 新增：动画状态判断 ---
        // 只要移动速度大于 0.1，就认为是在跑
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;

        // 把这个状态传给 Animator 里的 "isRunning" 开关
        // 如果 anim 没找到(比如你忘了加 Animator)，这行会报错，所以加个判断
        if (anim != null)
        {
            anim.SetBool("isRunning", isMoving);
        }

        // --- 原有的 AI 逻辑 ---
        if (playerScript.IsHidden)
        {
            agent.ResetPath();
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }
}