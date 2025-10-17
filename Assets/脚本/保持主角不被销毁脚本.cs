using UnityEngine;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence instance;

    private void Awake()
    {
        // 這是我們的單例模式檢查
        if (instance == null)
        {
            // 如果 instance 是空的，代表這是第一個 (也應是唯一一個) 玩家物件
            instance = this;
            // 告訴 Unity，在載入新場景時，不要銷毀這個物件
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果 instance 已經存在，代表場景中已經有一個持久化的玩家了
            // 這個新載入的玩家是多餘的，必須被銷毀，以避免出現兩個玩家
            Destroy(gameObject);
        }
    }
}