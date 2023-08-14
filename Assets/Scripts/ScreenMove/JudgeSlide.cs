using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class JudgeSlide : MonoBehaviour
{
    // Start is called before the first frame update
 

    // Update is called once per frame
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    // 最小滑动距离（单位：像素）
    public float minSwipeDistance = 20f;
    public UnityEvent Up;
    public UnityEvent Down;
    public UnityEvent Left;
    public UnityEvent Right;
    void Start()
    {

    }
    void Update()
    {
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                fingerDownPosition = touch.position;
                fingerUpPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                fingerUpPosition = touch.position;
                CheckSwipeGesture();
            }
        }
    }

    private void CheckSwipeGesture()
    {
        float swipeDistance = Vector2.Distance(fingerDownPosition, fingerUpPosition);

        // 检查滑动距离是否超过最小滑动距离
        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDirection = fingerUpPosition - fingerDownPosition;
            swipeDirection.Normalize();

            // 根据滑动方向进行相应操作
            if (swipeDirection.x > 0)
            {
                Debug.Log("向右滑动");
                Right?.Invoke();
            }
            else if (swipeDirection.x < 0)
            {
                Debug.Log("向左滑动");
                Left?.Invoke();
            }
            if (swipeDirection.y > 0)
            {
                Debug.Log("向上滑动");
                Up?.Invoke();
            }
            else if (swipeDirection.y < 0)
            {
                Debug.Log("向下滑动");
                Down?.Invoke();
            }
        }
    }
}
