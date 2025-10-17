using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator; // 新增：Animator 組件的引用
    private Vector2 movement;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // 獲取 Animator 組件
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            animator.SetBool("IsMoving", false); // 禁用移動時，將 IsMoving 設為 false
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");

        // 根據移動方向翻轉角色 Sprite
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // 向右，保持原樣
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1); // 向左，水平翻轉
        }

        // 更新 Animator 中的 IsMoving 參數
        bool isMoving = movement.x != 0;
        animator.SetBool("IsMoving", isMoving);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool status)
    {
        canMove = status;
        if (!status)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsMoving", false); // 禁用移動時，確保動畫停止
        }
    }
}