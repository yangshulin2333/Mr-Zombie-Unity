using UnityEngine;
using UnityEngine.AI; // 📢 注意：必须引入这个，才能用 NavMeshAgent

public class ZombieController : MonoBehaviour
{
    [Header("僵尸设置")]
    public float chaseSpeed = 3.5f; // 追击速度

    private NavMeshAgent agent;   // 僵尸的“腿” (自动寻路组件)
    private Transform player;     // 主角的位置
    private PlayerController playerScript; // 主角的脚本 (用来查屏息状态)

    void Start()
    {
        // 1. 获取自己的寻路组件
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        // 2. 自动在场景里找主角
        // FindAnyObjectByType 会自动找到挂了 PlayerController 的物体
        var p = FindAnyObjectByType<PlayerController>();
        if (p != null)
        {
            player = p.transform;
            playerScript = p;
        }
    }

    void Update()
    {
        // 如果没找到主角，就不动
        if (player == null) return;

        // --- 核心 AI 逻辑 ---

        // 问主角：你现在是藏着的吗？(屏息了吗？)
        if (playerScript.IsHidden)
        {
            // 情况A：玩家屏息了 -> 僵尸失去目标
            // 逻辑：原地停下，假装听不见
            agent.ResetPath();
        }
        else
        {
            // 情况B：玩家在呼吸 -> 僵尸听到声音 -> 追！
            // SetDestination 会自动计算绕过障碍物的最短路径
            agent.SetDestination(player.position);
        }
    }
}