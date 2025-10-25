using UnityEngine;

public class GlitchController : MonoBehaviour
{
    private Material glitchMaterial;
    private float targetIntensity = 0f;
    private float currentIntensity = 0f;

    void Start()
    {
        // 获取 SpriteRenderer 上的材质实例
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && renderer.material != null)
        {
            // 我们需要获取材质的实例，而不是共享材质
            glitchMaterial = renderer.material; 
        }
    }

    // Fungus 将调用这个方法来 "开启" 花屏
    public void StartGlitch()
    {
        Debug.Log("StartGlitch");
        // 你可以设置一个固定值，比如 1.0f
        if (glitchMaterial != null)
        {
            // "_GlitchIntensity" 必须与你在 Shader Graph 中创建的属性名完全一致
            glitchMaterial.SetFloat("_GlitchIntensity", 1.0f); 
        }
    }

    // Fungus 将调用这个方法来 "关闭" 花屏
    public void StopGlitch()
    {
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat("_GlitchIntensity", 0.0f);
        }
    }

    // (可选) 如果你希望效果是随机闪烁的，可以在 Update 中做文章
    // void Update()
    // {
    //     // 比如，让强度随机变化
    //     if (glitchMaterial != null && targetIntensity > 0)
    //     {
    //         currentIntensity = Random.Range(0.5f, 1.0f);
    //         glitchMaterial.SetFloat("_GlitchIntensity", currentIntensity);
    //     }
    // }
    //
    // public void StartGlitch() { targetIntensity = 1.0f; }
    // public void StopGlitch() 
    // { 
    //     targetIntensity = 0.0f; 
    //     glitchMaterial.SetFloat("_GlitchIntensity", 0.0f); 
    // }
}