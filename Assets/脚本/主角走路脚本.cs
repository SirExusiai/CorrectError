using UnityEngine;
using System.Collections; // 必须添加，用于协程

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

        // --- 【【在 Awake 中获取组件】】 ---
        // (放在 Start 之前更安全)
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }
    
    private Coroutine autoWalkCoroutine; // 存储自动走路的协程

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        // 只有当 canMove 为 true 时，才接收玩家输入
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

            // 动画逻辑
            animator.SetBool("IsMoving", movement.x != 0);
        }
    }

    void FixedUpdate()
    {
        // 无论如何（玩家控制或自动走路），都使用 movement 变量来移动
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool status) 
    {
        canMove = status;
        
        // 如果我们允许玩家移动，就必须停止所有自动走路
        if (status && autoWalkCoroutine != null)
        {
            StopCoroutine(autoWalkCoroutine);
            autoWalkCoroutine = null;
        }

        // 如果玩家被禁用，停止所有来自玩家的移动
        if (!status)
        {
            movement = Vector2.zero; // 停止玩家的移动
            rb.velocity = Vector2.zero;
            animator.SetBool("IsMoving", false); 
        }
    }

    // --- 新增的自动走路方法 (Fungus 将调用这个) ---
    
    /// <summary>
    /// 命令角色自动走到一个目标点 (只关心X轴)
    /// </summary>
    /// <param name="target">目标点的 Transform</param>
    public void AutoWalkTo(Transform target)
    {
        // 1. 禁用玩家输入
        SetCanMove(false);

        // 2. 停止任何可能正在运行的旧协程
        if (autoWalkCoroutine != null)
        {
            StopCoroutine(autoWalkCoroutine);
        }

        // 3. 启动新的自动走路协程
        autoWalkCoroutine = StartCoroutine(WalkToCoroutine(target.position));
    }

    private IEnumerator WalkToCoroutine(Vector2 targetPosition)
    {
        // 持续循环，直到 X 轴位置非常接近目标
        while (Mathf.Abs(rb.position.x - targetPosition.x) > 0.1f)
        {
            // 1. 计算方向 (1 或 -1)
            float directionX = Mathf.Sign(targetPosition.x - rb.position.x);
            
            // 2. 设置 movement 变量 (FixedUpdate 会使用它来移动)
            movement = new Vector2(directionX, 0);

            // 3. 手动处理翻转
            if (movement.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            // 4. 手动设置动画
            animator.SetBool("IsMoving", true);

            yield return null; // 等待下一帧
        }

        // 5. 到达目的地后，停止一切
        movement = Vector2.zero;
        animator.SetBool("IsMoving", false);
        rb.position = new Vector2(targetPosition.x, rb.position.y); // 强制对齐 X 轴
        autoWalkCoroutine = null; // 清空协程
    }
}