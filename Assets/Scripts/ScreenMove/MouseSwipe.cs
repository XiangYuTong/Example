using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseSwipe : MonoBehaviour
{
    // 鼠标左右滑动的最小移动量
    public float minSwipeDistance = 100f;

    // 左滑事件
    public UnityEngine.Events.UnityEvent onSwipeLeft;

    // 右滑事件
    public UnityEngine.Events.UnityEvent onSwipeRight;

    private Vector2 swipeStartPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeEndPosition = Input.mousePosition;
            float swipeDistance = swipeEndPosition.x - swipeStartPosition.x;

            if (Mathf.Abs(swipeDistance) >= minSwipeDistance)
            {
                if (swipeDistance < 0f)
                {
                    // 执行左滑事件
                    onSwipeLeft.Invoke();
                }
                else
                {
                    // 执行右滑事件
                    onSwipeRight.Invoke();
                }
            }
        }
    }
}

