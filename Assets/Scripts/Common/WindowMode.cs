//using Running.ReadSetting;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowMode : MonoBehaviour
{
    private static WindowMode windowMode;
    #region  WinAPI
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

    private const uint SWP_SHOWWINDOW = 64u;

    private const uint SWP_NOZORDER = 4u;

    private const int GWL_STYLE = -16;

    private const int WS_BORDER = 1;

    private const int WS_POPUP = 8388608;

    private const int WS_NOBORDER = 268435456;

    public static int SW_RESTORE = 9;
    public static int SW_MINIMIZE = 6;
    private const int SW_HIDE = 0;  //隐藏任务栏

    private static int HWND_TOPMOST = -1;
    private static int HWND_TOP = 0;
    #endregion
    public static WindowMode Instance
    {
        get
        {
            if (windowMode == null)
            {
                WindowMode ins = FindObjectOfType<WindowMode>();
                if (ins == null)
                {
                    Debug.LogError("场景中没有UIMgr组件，请添加");
                }
                else
                {
                    windowMode = ins;
                    Init();
                }
            }
            return windowMode;
        }
    }
    private static void Init()
    {
        Screen.SetResolution(Common.width, Common.height, false);
    }
    public void SetWindowMode()
    {
        if (Common.max == 2)
        {
            SetWindowLater();
            
        }
        else if (Common.max == 3)
        {
            SetWindowModeLoop();
        }
    }
    private void SetWindowLater()
    {
        StartCoroutine(DoWindowLater());
    }
    private IEnumerator DoWindowLater()
    {

        yield return new WaitForSeconds(0.1f);
        UpdateWindowMode();
        if (Common.startmode == 0)//最小化启动
        {
            yield return new WaitForSeconds(1f);
            HideApp();
        }

    }
    /// <summary>
    /// 窗体重复定位到指定位置
    /// 使用情况，在有些融合，led等信号传输过程中存在不稳的情况导致初始化的窗体位置发生变化
    /// </summary>
    private void SetWindowModeLoop()
    {
        StartCoroutine(DoWindowLoop());
    }
    /// <summary>
    /// 协程 10秒重新定位一次
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoWindowLoop()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            UpdateWindowMode();
            yield return new WaitForSeconds(10);
        }
    }

    private void UpdateWindowMode()
    {
        if (!Application.isEditor)
        {
            Cursor.visible = Convert.ToBoolean(Common.hidecursor);//隐藏光标
            IntPtr intPtr = FindWindow(null, Application.productName);
            SetWindowLong(intPtr, -16, 268435456);
            SetWindowPos(intPtr, new IntPtr(Convert.ToBoolean(Common.topmost) ? HWND_TOPMOST : HWND_TOP), Common.left, Common.top, Common.width, Common.height, 64u);
            if (Common.hidetaskbar == 1)//隐藏任务栏
            {
                HideTaskBar();
            }
            else
            {
                ShowTaskbar();
            }
        }

    }
    /// <summary>
    /// 显示APP窗体
    /// </summary>
    public void ShowApp()
    {
        if (!Application.isEditor)
        {
            IntPtr hwnd = FindWindow(null, Application.productName);
            ShowWindow(hwnd, SW_RESTORE);
        }
    }
    /// <summary>
    /// 最小化窗体
    /// </summary>
    public void HideApp()
    {
        if (!Application.isEditor)
        {
            IntPtr hwnd = FindWindow(null, Application.productName);
            ShowWindow(hwnd, SW_MINIMIZE);
        }
    }
    /// <summary>
    /// 显示任务栏
    /// </summary>
    public void ShowTaskbar()
    {
        if (!Application.isEditor)
        {
            ShowWindow(FindWindow("Shell_TrayWnd", null), SW_RESTORE);
            ShowWindow(FindWindow("Button", null), SW_RESTORE);
        }

    }
    /// <summary>
    /// 隐藏任务栏
    /// </summary>
    public void HideTaskBar()
    {
        
        if (!Application.isEditor)
        {
            ShowWindow(FindWindow("Shell_TrayWnd", null), SW_HIDE);
            ShowWindow(FindWindow("Button", null), SW_HIDE);
        }
    }
}
