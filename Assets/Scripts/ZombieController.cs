using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    [Header("僵尸设置")]
    public float chaseSpeed = 3.5f;

    private NavMeshAgent agent;
    private Transform player;
    private PlayerController playerScript;

    // 动画组件
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        // 获取子物体里的动画机
        anim = GetComponentInChildren<Animator>();

        // 自动寻找主角
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

        // --- 1. 动画逻辑 ---
        if (anim != null)
        {
            // 只要有速度，就播放跑动画
            bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
            anim.SetBool("isRunning", isMoving);
        }

        // --- 2. AI 追击逻辑 (包含屏息判断) ---
        // 如果主角藏起来了(IsHidden)，僵尸就停下发呆
        if (playerScript != null && playerScript.IsHidden)
        {
            agent.ResetPath(); // 停止寻路
        }
        else
        {
            agent.SetDestination(player.position); // 继续追
        }
    }

    // 👇👇👇 Day 5 新增：咬人判定 👇👇👇
    private void OnTriggerEnter(Collider other)
    {
        // 如果撞到的东西标签是 "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log(">>> 咬到你了！游戏结束！💀");

            // 暂停游戏
            Time.timeScale = 0;
        }
    }
}