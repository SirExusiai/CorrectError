using UnityEngine;
        
        // 这个脚本确保附加到的游戏对象在加载新场景时不会被销毁。
        // This script ensures that the GameObject it is attached to
        // is not destroyed when a new scene is loaded.
        public class MakeDontDestroy : MonoBehaviour
        {
            public static MakeDontDestroy Instance;
        
            void Awake()
            {
                // 实现单例模式 (Singleton Pattern)
                if (Instance == null)
                {
                    // 如果这是第一个实例，将它设为实例
                    Instance = this;
                    // 标记为“跨场景保留”
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    // 如果一个实例已经存在，则销毁这个重复的对象
                    Destroy(gameObject);
                }
            }
        }