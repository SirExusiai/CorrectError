using UnityEngine;

// 这是一个专门用于被“动画事件”调用的脚本。
// 它只负责提供一个公共函数来播放音效。

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider2D))] // 我们仍然需要它来进行地面检测
public class PlayerAudio : MonoBehaviour
{
    // --- 内部组件 ---
    private AudioSource audioSource;
    private Collider2D col2d;

    [Header("脚步声音频")]
    [Tooltip("存放所有可能的脚步声音频。脚本会从中随机抽取一个播放。")]
    [SerializeField]
    private AudioClip[] footstepClips;

    [Header("地面检测")]
    [Tooltip("必须设置！选择代表“地面”的图层。")]
    [SerializeField]
    private LayerMask groundLayer;

    void Awake()
    {
        // 获取组件
        audioSource = GetComponent<AudioSource>();
        col2d = GetComponent<Collider2D>();
    }

    // ==========================================================
    // ==           !!! 这是关键的公共函数 !!!           ==
    // ==========================================================
    // 
    // 这个函数将被你的“行走”动画中的“动画事件” (Animation Event) 调用。
    // 它必须是 public void，并且没有参数。
    //
    public void PlayFootstepSound()
    {
        // --- 1. 检查是否在地面上 ---
        // 我们不希望在跳跃或坠落时还播放脚步声
        if (!col2d.IsTouchingLayers(groundLayer))
        {
            // 在空中，直接返回，不播放声音
            return; 
        }

        // --- 2. 检查是否有可播放的音频 ---
        if (footstepClips == null || footstepClips.Length == 0)
        {
            Debug.LogWarning("PlayerAudio 脚本中没有设置脚步声音频 (footstepClips)！");
            return;
        }

        // --- 3. 随机播放一个音效 ---
        // 从数组中随机选一个
        int index = Random.Range(0, footstepClips.Length);
        AudioClip clipToPlay = footstepClips[index];
        
        // 使用 PlayOneShot 播放，这样音效可以重叠，听起来更自然
        audioSource.PlayOneShot(clipToPlay);
    }
}
