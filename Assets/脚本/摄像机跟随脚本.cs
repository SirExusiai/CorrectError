using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // 我們不再需要從 Inspector 中手動設定 target，所以可以將它設為 private
    private Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    public float minX;
    public float maxX;

    // 我們使用 Start() 函式，它在物件第一次被啟用時執行
    void Start()
    {
        // 嘗試自動尋找玩家
        FindPlayer();
    }

    void LateUpdate()
    {
        // 如果 target 仍然是 null (例如在主選單等沒有玩家的場景)，就直接返回
        if (target == null)
        {
            // 我們可以再次嘗試尋找，以應對玩家稍後才出現的情況
            FindPlayer();
            if (target == null) return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        smoothedPosition.y = transform.position.y;
        smoothedPosition.z = transform.position.z;

        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);

        transform.position = smoothedPosition;
    }

    // 這是一個新的函式，專門用來尋找玩家
    private void FindPlayer()
    {
        // 透過我們建立的 PlayerPersistence 單例來安全地找到玩家
        if (PlayerPersistence.instance != null)
        {
            target = PlayerPersistence.instance.transform;
            Debug.Log("攝影機已自動鎖定目標: " + target.name);
        }
        else
        {
            Debug.LogWarning("攝影機在場景中找不到玩家的 PlayerPersistence 實例。");
        }
    }
}