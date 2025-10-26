using UnityEngine;
using Fungus; // 引用Fungus命名空间

public class DragController : MonoBehaviour
{
    // 获取Draggable2D组件的引用
    private Draggable2D draggable;

    void Awake()
    {
        // 在游戏开始时找到这个组件
        draggable = GetComponent<Draggable2D>();
    }

    // 创建一个公开的方法来禁用拖拽
    public void DisableDragging()
    {
        if (draggable != null)
        {
            draggable.DragEnabled = false;
        }
    }

    // 创建一个公开的方法来启用拖拽
    public void EnableDragging()
    {
        if (draggable != null)
        {
            draggable.DragEnabled = true;
        }
    }
}