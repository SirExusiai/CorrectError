using UnityEngine;
using Fungus; // 1. 引入 Fungus 命名空间

// 2. 确保该游戏对象有一个 Collider2D 组件，这是 OnMouseDown 工作的前提
[RequireComponent(typeof(Collider2D))]
public class ClickSpriteToTriggerBlock : MonoBehaviour
{
    [Header("Fungus Settings")] // 在 Inspector 中添加标题，更清晰

    [Tooltip("将包含目标 Block 的 Fungus Flowchart 游戏对象拖拽到这里")] // 3. 公开变量，用于在 Inspector 中链接 Flowchart
    public Flowchart targetFlowchart;

    [Tooltip("输入想要在点击时执行的 Block 的确切名字 (大小写敏感)")] // 4. 公开变量，用于在 Inspector 中指定 Block 名称
    public string targetBlockName;

    // 5. OnMouseDown 是 Unity 的内置函数
    // 当鼠标左键点击附加到此游戏对象的 Collider2D 时，此函数会自动被调用
    private void OnMouseDown()
    {
        // 6. 安全检查：确保 Flowchart 和 Block 名称都已设置
        if (targetFlowchart == null)
        {
            Debug.LogError("错误：目标 Flowchart 未在 " + gameObject.name + " 的 ClickSpriteToTriggerBlock 脚本中设置！");
            return; // 如果未设置 Flowchart，则停止执行
        }

        if (string.IsNullOrEmpty(targetBlockName))
        {
            Debug.LogError("错误：目标 Block 名称未在 " + gameObject.name + " 的 ClickSpriteToTriggerBlock 脚本中设置！");
            return; // 如果未设置 Block 名称，则停止执行
        }

        // 7. 执行 Fungus Block
        Debug.Log("点击精灵 " + gameObject.name + ", 准备执行 Block: " + targetBlockName);
        targetFlowchart.ExecuteBlock(targetBlockName);
    }

    // --- 可选：添加一些视觉反馈 ---

    // 当鼠标悬停在 Collider2D 上方时调用
    private void OnMouseEnter()
    {
        // 可以在这里添加一些视觉提示，比如改变鼠标指针、让精灵轻微发光或放大
        // 例如: transform.localScale = Vector3.one * 1.1f; // 稍微放大
        // 例如: GetComponent<SpriteRenderer>().color = Color.yellow; // 变黄 (仅示例)
        // 你可能需要一个更复杂的系统来改变鼠标指针
    }

    // 当鼠标从 Collider2D 上方移开时调用
    private void OnMouseExit()
    {
        // 恢复视觉提示
        // 例如: transform.localScale = Vector3.one; // 恢复原始大小
        // 例如: GetComponent<SpriteRenderer>().color = Color.white; // 恢复白色 (仅示例)
    }
}