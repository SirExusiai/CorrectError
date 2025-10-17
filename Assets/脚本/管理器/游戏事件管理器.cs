using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 確保引用了 SceneManagement

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance;
    public static string nextEntryPointName; // 靜態變數，用來儲存下一個入口點的名稱

    // 使用 Dictionary 來儲存所有的遊戲旗標，string 是旗標的名字，bool 是它的狀態 (true/false)
    private Dictionary<string, bool> eventFlags = new Dictionary<string, bool>();

    void Awake()
    {
        // 單例模式設定，確保場景中只有一個 GameEventManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 確保切換場景時，這個管理器不會被銷毀
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        // 訂閱場景載入完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 取消訂閱，避免記憶體洩漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // 當一個新場景載入完成後，這個函式會被自動呼叫
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 首先，在新場景中尋找場景資訊
        SceneInfo sceneInfo = FindObjectOfType<SceneInfo>();

        // 獲取玩家物件的引用
        GameObject playerObject = null;
        if (PlayerPersistence.instance != null)
        {
            playerObject = PlayerPersistence.instance.gameObject;
        }

        if (playerObject == null) return; // 如果找不到玩家，就直接返回

        // 根據場景資訊來決定是否啟用玩家
        if (sceneInfo != null && !sceneInfo.hasPlayer)
        {
            // 如果場景資訊說「沒有玩家」，就禁用玩家物件
            playerObject.SetActive(false);
            Debug.Log("進入無玩家場景，已禁用玩家。");
            return; // 結束後續處理
        }
    
        // --- 如果程式碼執行到這裡，代表這是一個需要玩家的場景 ---

        // 確保玩家是啟用的
        playerObject.SetActive(true);
        Debug.Log("進入玩家場景，已啟用玩家。");
        if (!string.IsNullOrEmpty(nextEntryPointName))
        {
            // 在新場景中尋找所有 EntryPoint
            EntryPoint[] entryPoints = FindObjectsOfType<EntryPoint>();
            foreach (var entryPoint in entryPoints)
            {
                if (entryPoint.entryName == nextEntryPointName)
                {
                    // 找到了匹配的入口！
                    // 找到玩家物件並將其移動到入口點的位置
                    if (PlayerPersistence.instance != null)
                    {
                        PlayerPersistence.instance.transform.position = entryPoint.transform.position;
                        Debug.Log("玩家已移動到入口: " + nextEntryPointName);
                    }
                    break; // 停止搜尋
                }
            }
            // 清空名稱，為下一次傳送做準備
            nextEntryPointName = null;
        }
    }

    // 設定一個旗標的狀態
    public void SetFlag(string flagName, bool value)
    {
        // 如果旗標已存在，就更新它的值；如果不存在，就新增它
        if (eventFlags.ContainsKey(flagName))
        {
            eventFlags[flagName] = value;
        }
        else
        {
            eventFlags.Add(flagName, value);
        }
        Debug.Log("旗標 '" + flagName + "' 已被設定為 " + value);
    }

    // 檢查一個旗標的狀態 (如果旗標從未被設定過，預設返回 false)
    public bool GetFlag(string flagName)
    {
        if (eventFlags.ContainsKey(flagName))
        {
            return eventFlags[flagName];
        }
        else
        {
            return false;
        }
    }

    // 檢查多個旗標是否全部為 true
    public bool AreAllFlagsSet(string[] flagNames)
    {
        foreach (string flagName in flagNames)
        {
            if (!GetFlag(flagName)) // 只要有一個旗標是 false
            {
                return false; // 立刻返回 false
            }
        }
        return true; // 所有旗標都是 true
    }

    // 檢查多個旗標是否全部為 false
    public bool AreAllFlagsUnset(string[] flagNames)
    {
        foreach (string flagName in flagNames)
        {
            if (GetFlag(flagName)) // 只要有一個旗標是 true
            {
                return false; // 立刻返回 false
            }
        }
        return true; // 所有旗標都是 false
    }
}