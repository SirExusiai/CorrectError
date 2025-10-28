using UnityEngine;
using System.Collections;

// 【【【 这是一个已修复的版本 】】】
// 1. 使用 rb.velocity 来移动，更平滑
// 2. 使用 rb.velocity 来设置动画，解决“平移/卡顿”问题
// 3. 移除了协程中的手动动画控制，防止冲突

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public static PlayerMovement Instance { get; private set; }
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private bool canMove = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // (移除了多余的 Start() )

    void Update()
    {
        // 1. 只有当 canMove 为 true 时，才接收玩家输入
        if (canMove)
        {
            movement.x = Input.GetAxisRaw("Horizontal");

            // 翻转逻辑
            if (movement.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        
        // 2. 【【【 关键修复 】】】
        // 总是根据刚体(Rigidbody)的“实际速度”来设置动画
        // 我们不再关心 'movement.x'，只关心 'rb.velocity.x'
        // Mathf.Abs() 是取绝对值
        animator.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    void FixedUpdate()
    {
        // 【【【 关键修复 】】】
        // 使用 rb.velocity 来移动，而不是 MovePosition。
        // 这能让物理引擎正确计算速度，也让动画同步。
        // (我们保留 rb.velocity.y 以便未来加入跳跃或重力)
        rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
    }

    public void SetCanMove(bool status)
    {
        canMove = status;

        if (status && autoWalkCoroutine != null)
        {
            StopCoroutine(autoWalkCoroutine);
            autoWalkCoroutine = null;
        }

        if (!status)
        {
            movement = Vector2.zero; 
            // (rb.velocity 会在下一次 FixedUpdate 中自动变为 0)
            // (animator 会在下一次 Update 中自动变为 false)
        }
    }

    // --- 自动走路协程 (已简化) ---
    
    private Coroutine autoWalkCoroutine; 

    public void AutoWalkTo(Transform target)
    {
        SetCanMove(false);

        if (autoWalkCoroutine != null)
        {
            StopCoroutine(autoWalkCoroutine);
        }
        autoWalkCoroutine = StartCoroutine(WalkToCoroutine(target.position));
    }

    private IEnumerator WalkToCoroutine(Vector2 targetPosition)
    {
        while (Mathf.Abs(rb.position.x - targetPosition.x) > 0.1f)
        {
            float directionX = Mathf.Sign(targetPosition.x - rb.position.x);
            movement = new Vector2(directionX, 0); // 设置 movement

            // 手动处理翻转 (这仍然需要)
            if (movement.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            // 【【【 关键修复 】】】
            // 移除了这里手动的 animator.SetBool("IsMoving", true);
            // Update() 中的逻辑会自动处理它

            yield return null; 
        }

        // 5. 到达目的地后，停止一切
        movement = Vector2.zero;
        rb.position = new Vector2(targetPosition.x, rb.position.y); 
        autoWalkCoroutine = null; 
        
        // (Update() 会自动设置 IsMoving 为 false)
    }
}
