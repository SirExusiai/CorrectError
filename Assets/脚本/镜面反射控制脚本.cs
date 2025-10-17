using UnityEngine;

// 将此脚本挂载到您的主摄像机 (Main Camera) 上
public class ManualMirrorReflection : MonoBehaviour
{
    [Tooltip("场景中的镜子物体")]
    public Transform mirrorPlane;

    [Tooltip("用于反射的摄像机")]
    public Camera reflectionCamera;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        // 我们将手动控制渲染，所以默认禁用反射摄像机，防止它自动渲染
        if (reflectionCamera != null)
        {
            reflectionCamera.enabled = false;
        }
    }

    // 使用 LateUpdate 确保在玩家和主摄像机都完成移动后再执行
    void LateUpdate()
    {
        if (mirrorPlane == null || mainCamera == null || reflectionCamera == null)
        {
            return;
        }

        // --- 1. 计算并设置反射摄像机的位置 ---
        // 获取主摄像机当前帧的位置
        Vector3 mainCamPos = mainCamera.transform.position;
        // 计算主摄像机与镜子中心点的X轴距离
        float xDistance = mainCamPos.x - mirrorPlane.position.x;

        // 将反射摄像机设置在与主摄像机X轴对称的位置，Y轴保持一致
        reflectionCamera.transform.position = new Vector3(
            mirrorPlane.position.x - xDistance,
            mainCamPos.y,
            reflectionCamera.transform.position.z // Z轴保持反射摄像机自己的深度
        );

        // --- 2. 手动命令反射摄像机渲染 ---
        // 在设置好位置之后，立刻命令反射摄像机渲染一帧画面到它的 Target Texture
        reflectionCamera.Render();
    }
}