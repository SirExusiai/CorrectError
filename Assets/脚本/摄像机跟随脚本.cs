using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 要跟隨的目標 (玩家)
    public float smoothSpeed = 0.125f; // 攝影機跟隨的平滑度
    public Vector3 offset; // 攝影機與目標的偏移量

    // 可以在這裡設定場景的邊界
    public float minX;
    public float maxX;

    void LateUpdate()
    {
        if (target == null) return;

        // 計算目標位置
        Vector3 desiredPosition = target.position + offset;
        // 使用 Lerp 達成平滑跟隨
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 將攝影機的 Y 和 Z 軸鎖定，只跟隨 X 軸
        smoothedPosition.y = transform.position.y;
        smoothedPosition.z = transform.position.z;

        // 套用邊界限制
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);

        transform.position = smoothedPosition;
    }
}