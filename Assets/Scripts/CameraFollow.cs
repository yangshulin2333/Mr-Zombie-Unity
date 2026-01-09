using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target; // 要跟谁？(把主角拖进来)

    [Header("平滑设置")]
    public float smoothSpeed = 5f; // 跟随有多“滑”？(数值越大跟得越紧)

    private Vector3 offset; // 摄像机和主角之间的固定距离

    void Start()
    {
        // 1. 自动计算偏移量
        // 游戏一开始，先算一下摄像机离主角现在的距离是多少，以后就保持这个距离
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    // 📢 面试考点：为什么要用 LateUpdate？
    // Update: 主角在移动
    // LateUpdate: 每一帧所有Update执行完后，摄像机再跟过去。
    // 如果都在Update里，可能会出现主角动了但摄像机还没动导致的“画面抖动”。
    void LateUpdate()
    {
        if (target == null) return;

        // 2. 计算目标位置
        Vector3 targetPosition = target.position + offset;

        // 3. 平滑移动 (Lerp)
        // 让摄像机不是瞬间瞬移，而是像橡皮筋一样平滑地飘过去，更有高级感
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}