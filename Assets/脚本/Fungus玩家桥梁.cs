using UnityEngine;

// 这个脚本是“中间人”。
// Fungus 将调用这个脚本中的方法。
// 这个脚本会找到那个持久化的“真主角”(Singleton)，并把命令传给它。

public class PlayerFungusBridge : MonoBehaviour
{
    private PlayerMovement realPlayer;
    private Animator playerAnimator;

    // 尝试获取一次主角引用
    private void FindRealPlayer()
    {
        if (realPlayer == null)
        {
            // 通过 Singleton (单例) 模式，立即找到那个持久化的主角
            realPlayer = PlayerMovement.Instance;

            if (realPlayer != null)
            {
                // 顺便获取它的 Animator
                playerAnimator = realPlayer.GetComponent<Animator>();
            }
        }
    }

    // --- Fungus 将调用这些 Public 方法 ---

    // Fungus 调用这个来禁用玩家输入
    public void SetCanMove(bool status)
    {
        FindRealPlayer(); // 确保我们有主角的引用
        if (realPlayer != null)
        {
            realPlayer.SetCanMove(status);
        }
    }

    // Fungus 调用这个来设置动画 (解决 Set Bool 无法传参的问题)
    public void SetPlayerAnimatorBool_IsMoving_True()
    {
        FindRealPlayer();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsMoving", true);
        }
    }
    
    // Fungus 调用这个来设置动画 (解决 Set Bool 无法传参的问题)
    public void SetPlayerAnimatorBool_IsMoving_False()
    {
        FindRealPlayer();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsMoving", false);
        }
    }
    public void CallAutoWalkTo(Transform targetPosition)
    {
        FindRealPlayer(); // 确保我们有主角的引用
        if (realPlayer != null)
        {
            // 将命令传递给真正的主角
            realPlayer.AutoWalkTo(targetPosition);
        }
    }
    
    // (如果你需要调用其他动画参数，可以按上面的格式添加更多方法)
}