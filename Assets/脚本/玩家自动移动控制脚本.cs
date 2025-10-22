using UnityEngine;
using System.Collections;
using Fungus; // 引用 Fungus 命名空间

// 这个脚本现在可以挂载在场景中的任何 GameObject 上，例如触发器本身，或者一个专门的管理对象
public class AutoMoveSequence : MonoBehaviour
{
    // --- 在 Inspector 中设置的变量 ---
    [Tooltip("包含对话 Block 和主角 Transform 全局变量的 Fungus Flowchart")]
    public Flowchart targetFlowchart; // 将场景中的 Fungus Flowchart 拖拽到这里

    [Tooltip("移动速度")]
    public float moveSpeed = 5f; // 可以根据需要调整自动移动的速度

    // --- 内部状态 ---
    private bool isAutoMoving = false; // 防止重复触发

    void Start()
    {
        if (targetFlowchart == null)
        {
            Debug.LogError("请在 Inspector 中指定 Target Flowchart!");
        }
    }

    // --- 辅助方法：通过全局变量名获取主角对象及其组件 ---

    /// <summary>
    /// 尝试从 Flowchart 获取主角的 Transform
    /// </summary>
    private Transform GetPlayerTransform(string transformVariableName)
    {
        if (targetFlowchart == null || string.IsNullOrEmpty(transformVariableName))
        {
            Debug.LogError("Flowchart 或 Transform 变量名无效!");
            return null;
        }

        Variable variable = targetFlowchart.GetVariable(transformVariableName);
        TransformVariable transformVariable = variable as TransformVariable;

        if (transformVariable == null)
        {
            Debug.LogError($"在 Flowchart 中找不到名为 '{transformVariableName}' 的 Transform 变量，或者变量类型不匹配。");
            return null;
        }

        if (transformVariable.Value == null)
        {
             Debug.LogError($"Flowchart 中的 Transform 变量 '{transformVariableName}' 未设置 (值为 null)。请确保在游戏开始时已将其指向主角对象。");
             return null;
        }

        return transformVariable.Value;
    }

    /// <summary>
    /// 尝试获取指定 Transform 上的组件
    /// </summary>
    private T GetComponentFromPlayer<T>(Transform playerTransform) where T : Component
    {
        if (playerTransform == null) return null;

        T component = playerTransform.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"在主角对象 '{playerTransform.name}' 上找不到组件: {typeof(T).Name}");
        }
        return component;
    }


    // --- 由 Fungus 调用的公共方法 ---

    /// <summary>
    /// 开始自动移动序列：禁用控制 -> 移动到位置1 -> 触发对话
    /// </summary>
    /// <param name="playerTransformVarName">Flowchart 中存储主角 Transform 的全局变量名</param>
    /// <param name="targetPosition1Str">目标位置1 (格式: "x,y,z")</param>
    /// <param name="dialogueBlockName">要执行的对话 Block 名称</param>
    public void StartSequence(string playerTransformVarName, string targetPosition1Str, string dialogueBlockName)
    {
        if (isAutoMoving || targetFlowchart == null)
        {
            return; // 如果正在移动或 Flowchart 未设置，则不执行
        }

        Transform playerTransform = GetPlayerTransform(playerTransformVarName);
        if (playerTransform == null)
        {
            Debug.LogError("无法获取主角 Transform，序列中止。");
            return;
        }
        // 获取主角走路脚本的引用，这里假设脚本名为 "主角走路脚本"
        MonoBehaviour playerMovementScript = GetComponentFromPlayer<MonoBehaviour>(playerTransform); // 尝试直接获取基类
        if (playerMovementScript != null && playerMovementScript.GetType().Name != "主角走路脚本") // 如果获取到了但名字不对，尝试精确查找
        {
             playerMovementScript = playerTransform.GetComponent("主角走路脚本") as MonoBehaviour;
        }

        if (playerMovementScript == null) {
             Debug.LogError("未能找到主角走路脚本 '主角走路脚本'，序列中止。");
             return;
        }


        Vector3 targetPosition1 = ParseVector3(targetPosition1Str);
        if (targetPosition1 == Vector3.zero && targetPosition1Str != "0,0,0") // 简单检查解析是否成功
        {
            Debug.LogError("目标位置1格式错误，请使用 'x,y,z' 格式: " + targetPosition1Str);
            return;
        }

        StartCoroutine(MoveToWardCoroutine(playerTransform, playerMovementScript, targetPosition1, dialogueBlockName));
    }

    /// <summary>
    /// 触发返回移动：移动到位置2 -> 恢复控制
    /// </summary>
    /// <param name="playerTransformVarName">Flowchart 中存储主角 Transform 的全局变量名</param>
    /// <param name="targetPosition2Str">目标位置2 (格式: "x,y,z")</param>
    public void TriggerReturnMove(string playerTransformVarName, string targetPosition2Str)
    {
         if (!isAutoMoving) // 确保是从 StartSequence 开始的
         {
             Debug.LogWarning("TriggerReturnMove 在未开始序列时被调用。");
             return;
         }

        Transform playerTransform = GetPlayerTransform(playerTransformVarName);
         if (playerTransform == null)
         {
             Debug.LogError("无法获取主角 Transform，无法执行返回移动。可能需要手动恢复玩家控制。");
             // 尝试在不知道玩家对象的情况下结束标志位，避免卡死
             isAutoMoving = false;
             return;
         }
         // 获取主角走路脚本的引用
         MonoBehaviour playerMovementScript = GetComponentFromPlayer<MonoBehaviour>(playerTransform);
          if (playerMovementScript != null && playerMovementScript.GetType().Name != "主角走路脚本")
         {
              playerMovementScript = playerTransform.GetComponent("主角走路脚本") as MonoBehaviour;
         }
         // 即使找不到脚本，也要尝试移动和结束状态
         // if (playerMovementScript == null) return;


        Vector3 targetPosition2 = ParseVector3(targetPosition2Str);
         if (targetPosition2 == Vector3.zero && targetPosition2Str != "0,0,0")
        {
            Debug.LogError("目标位置2格式错误，请使用 'x,y,z' 格式: " + targetPosition2Str);
            // 即使格式错误，也尝试恢复控制
            EnablePlayerControl(playerMovementScript);
            isAutoMoving = false;
            return;
        }

        StartCoroutine(ReturnCoroutine(playerTransform, playerMovementScript, targetPosition2));
    }

    // --- 协程 ---

    /// <summary>
    /// 移动到病房位置并触发对话的协程
    /// </summary>
    IEnumerator MoveToWardCoroutine(Transform playerTransform, MonoBehaviour playerMovementScript, Vector3 targetPosition, string dialogueBlockName)
    {
        isAutoMoving = true;
        DisablePlayerControl(playerTransform, playerMovementScript);

        Animator playerAnimator = GetComponentFromPlayer<Animator>(playerTransform); // 在需要时获取 Animator

        // --- 移动到目标位置 1 ---
        yield return StartCoroutine(MoveCharacterTo(playerTransform, playerAnimator, targetPosition));

        // 停止移动动画 (如果播放了)
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsWalking", false);
            // playerAnimator.SetTrigger("StopWalk");
        }

        // --- 触发对话 Block ---
        if (targetFlowchart != null && !string.IsNullOrEmpty(dialogueBlockName))
        {
            targetFlowchart.ExecuteBlock(dialogueBlockName);
            // 注意: 这里不等待 Block 执行完成，让 Block 自己在最后调用 TriggerReturnMove
        }
        else
        {
             Debug.LogError("Flowchart 或 Dialogue Block 名称无效，无法触发对话！将直接尝试返回。");
              EnablePlayerControl(playerMovementScript);
              isAutoMoving = false;
        }
    }

    /// <summary>
    /// 返回起点的协程
    /// </summary>
    IEnumerator ReturnCoroutine(Transform playerTransform, MonoBehaviour playerMovementScript, Vector3 targetPosition)
    {
         Animator playerAnimator = GetComponentFromPlayer<Animator>(playerTransform); // 在需要时获取 Animator

        // --- 移动回目标位置 2 ---
        yield return StartCoroutine(MoveCharacterTo(playerTransform, playerAnimator, targetPosition));

         // 停止移动动画 (如果播放了)
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsWalking", false);
             // playerAnimator.SetTrigger("StopWalk");
        }

        // --- 恢复玩家控制 ---
        EnablePlayerControl(playerMovementScript);
        isAutoMoving = false;
    }

    /// <summary>
    /// 实际移动角色的协程 (使用 Lerp 平滑移动)
    /// </summary>
    IEnumerator MoveCharacterTo(Transform playerTransform, Animator playerAnimator, Vector3 targetPosition)
    {
        Vector3 startPosition = playerTransform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        if (distance < 0.01f) yield break; // 如果距离太近，直接返回

        float duration = distance / moveSpeed; // 根据速度和距离计算时间
        float elapsedTime = 0f;

         // 开始移动动画 (如果需要)
         UpdateFacingDirection(playerTransform, targetPosition - startPosition); // 更新朝向
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsWalking", true);
             // playerAnimator.SetTrigger("StartWalk");
        }

        while (elapsedTime < duration)
        {
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        playerTransform.position = targetPosition; // 确保精确到达
    }


    // --- 辅助方法 ---

    /// <summary>
    /// 禁用玩家控制脚本
    /// </summary>
    void DisablePlayerControl(Transform playerTransform, MonoBehaviour playerMovementScript)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;

             // 同时停止当前可能的速度
             Rigidbody2D rb = GetComponentFromPlayer<Rigidbody2D>(playerTransform);
             if(rb != null)
             {
                 rb.velocity = Vector2.zero;
             }
             // 停止走路动画
             Animator playerAnimator = GetComponentFromPlayer<Animator>(playerTransform);
             if (playerAnimator != null)
             {
                 playerAnimator.SetBool("IsWalking", false);
             }
        }
    }

    /// <summary>
    /// 启用玩家控制脚本
    /// </summary>
    void EnablePlayerControl(MonoBehaviour playerMovementScript)
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
         else
         {
              Debug.LogWarning("尝试启用玩家控制，但 playerMovementScript 为 null。");
         }
    }

     /// <summary>
    /// 根据移动方向更新角色朝向（简单示例，需要根据你的角色设置调整）
    /// </summary>
    void UpdateFacingDirection(Transform playerTransform, Vector3 moveDirection)
    {
        if (playerTransform == null) return;

        if (moveDirection.x > 0.01f) // 向右
        {
            playerTransform.localScale = new Vector3(Mathf.Abs(playerTransform.localScale.x), playerTransform.localScale.y, playerTransform.localScale.z);
        }
        else if (moveDirection.x < -0.01f) // 向左
        {
            playerTransform.localScale = new Vector3(-Mathf.Abs(playerTransform.localScale.x), playerTransform.localScale.y, playerTransform.localScale.z);
        }
    }


    /// <summary>
    /// 将 "x,y,z" 格式的字符串解析为 Vector3
    /// </summary>
    Vector3 ParseVector3(string sVector)
    {
        // ... (解析逻辑与之前相同，保持不变) ...
         if (string.IsNullOrEmpty(sVector)) return Vector3.zero;

        // 移除括号（如果有）
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // 按逗号分割
        string[] sArray = sVector.Split(',');

        // 检查是否有三个部分
        if (sArray.Length != 3)
        {
            Debug.LogError("无法将字符串解析为 Vector3: " + sVector + " - 需要3个逗号分隔的值。");
            return Vector3.zero;
        }

        // 尝试解析每个部分
        if (float.TryParse(sArray[0].Trim(), out float x) &&
            float.TryParse(sArray[1].Trim(), out float y) &&
            float.TryParse(sArray[2].Trim(), out float z))
        {
            return new Vector3(x, y, z);
        }
        else
        {
            Debug.LogError("无法将字符串解析为 Vector3: " + sVector + " - 值不是有效的浮点数。");
            return Vector3.zero;
        }
    }
}