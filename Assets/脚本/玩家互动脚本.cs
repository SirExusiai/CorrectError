using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 1.5f; // 玩家可以互動的距離
    public KeyCode interactionKey = KeyCode.E; // 互動按鍵

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        // 取得玩家的朝向 (基於 Scale 的 x 值判斷)
        float facingDirection = transform.localScale.x > 0 ? 1f : -1f;
        Vector2 rayDirection = new Vector2(facingDirection, 0);

        // 從玩家中心發射一條射線 (Raycast)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, interactionDistance);

        // 畫一條輔助線，方便在編輯器中看到射線的位置 (僅在 Scene 視窗可見)
        Debug.DrawRay(transform.position, rayDirection * interactionDistance, Color.red, 0.5f);

        // 檢查射線是否碰撞到任何東西
        if (hit.collider != null)
        {
            // 嘗試從碰撞到的物件上獲取 IInteractable 組件
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            // 如果成功獲取到 (表示這個物件是可互動的)
            if (interactable != null)
            {
                // 執行它的 Interact() 方法
                interactable.Interact();
            }
        }
    }
}