using UnityEngine;
using System.Collections;

public class SmoothSitTransition : MonoBehaviour
{
    [Header("站姿与坐姿对象")]
    public GameObject standObject;  // 当前站姿角色
    public GameObject sitObject;    // 坐姿角色（初始禁用）

    [Header("床位参考点")]
    public Transform bedTarget;

    [Header("淡入淡出参数")]
    [Range(0f, 2f)]
    public float fadeDuration = 0.5f;

    private CanvasGroup standFadeGroup;
    private CanvasGroup sitFadeGroup;

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip sitSound;

    // ✅ Fungus 中调用的方法（Method Name：TeleportToBedAndSit）
    public void TeleportToBedAndSit()
    {
        if (standObject == null || sitObject == null || bedTarget == null)
        {
            Debug.LogError("[SmoothSitTransition] 请确保所有引用都已设置！");
            return;
        }

        // 获取 CanvasGroup
        standFadeGroup = standObject.GetComponent<CanvasGroup>();
        sitFadeGroup = sitObject.GetComponent<CanvasGroup>();

        // 如果没有 CanvasGroup，就自动添加一个
        if (standFadeGroup == null) standFadeGroup = standObject.AddComponent<CanvasGroup>();
        if (sitFadeGroup == null) sitFadeGroup = sitObject.AddComponent<CanvasGroup>();

        // 坐姿初始为透明
        sitFadeGroup.alpha = 0f;
        sitObject.SetActive(true);

        // 开始过渡协程
        StartCoroutine(SwitchToSitPose());
    }

    private IEnumerator SwitchToSitPose()
    {
        // 1️⃣ 淡出站姿角色
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            standFadeGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        standFadeGroup.alpha = 0f;
        standObject.SetActive(false);

        // 2️⃣ 播放坐下音效
        if (audioSource != null && sitSound != null)
        {
            audioSource.PlayOneShot(sitSound);
        }

        // 3️⃣ 将坐姿角色移动到床上位置
        sitObject.transform.position = bedTarget.position;

        // 4️⃣ 淡入坐姿角色
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            sitFadeGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        sitFadeGroup.alpha = 1f;

        Debug.Log("[SmoothSitTransition] 角色已淡出站姿、播放音效并淡入坐姿。");
    }
}
