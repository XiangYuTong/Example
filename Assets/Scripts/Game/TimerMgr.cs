using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void Handler();
public delegate void Handler<T1>(T1 param1);
public delegate void Handler<T1, T2>(T1 param1, T2 param2);
public delegate void Handler<T1, T2, T3>(T1 param1, T2 param2, T3 param3);

public interface IAnimatable
{
    void AdvanceTime();
}

/**
 * ��Ҫ��һ�����ڴ��ڵ�Update�����ʼ��
 * foreach (IAnimatable animatable in TimerManager.timerList)
        {
            animatable.AdvanceTime();
        }
*/

/**ʱ�ӹ�����[ͬһ������μ�ʱ��Ĭ�ϻᱻ���߸���,delayС��1������ִ��]*/
public class TimerMgr : MonoBehaviour, IAnimatable
{

    public static List<IAnimatable> timerList = new List<IAnimatable>();

    private static TimerMgr _instance;//����

    public static TimerMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                TimerMgr ins = FindObjectOfType<TimerMgr>();
                if (ins == null)
                {
                    Debug.LogError("������û��TimerMgr������Ѿ��Զ�����");
                    GameObject go = new GameObject(nameof(TimerMgr));
                    go.transform.parent = GameManager.instance.transform;
                    _instance = go.AddComponent<TimerMgr>();
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
    }

    public void Init()
    {

    }
    private void Update()
    {
            foreach (IAnimatable animatable in timerList)
            {
                animatable.AdvanceTime();
            }
    }
    public TimerMgr()
    {
        timerList.Add(this);
    }

    private List<TimerHandler> _pool = new List<TimerHandler>();
    /** �����鱣֤������˳��ִ��*/
    private List<TimerHandler> _handlers = new List<TimerHandler>();
    private int _currFrame = 0;
    private uint _index = 0;

    public void AdvanceTime()
    {
        _currFrame++;
        for (int i = 0; i < _handlers.Count; i++)
        {
            TimerHandler handler = _handlers[i];
            long t = handler.userFrame ? _currFrame : currentTime;
            if (t >= handler.exeTime)
            {
                Delegate method = handler.method;
                object[] args = handler.args;
                if (handler.repeat)
                {
                    while (t >= handler.exeTime)
                    {
                        handler.exeTime += handler.delay;
                        method.DynamicInvoke(args);
                    }
                }
                else
                {
                    clear(handler.method);
                    method.DynamicInvoke(args);
                }
            }
        }
    }

    private object create(bool useFrame, bool repeat, int delay, Delegate method, params object[] args)
    {
        if (method == null)
        {
            return null;
        }

        //���ִ��ʱ��С��1��ֱ��ִ��
        if (delay < 1)
        {
            method.DynamicInvoke(args);
            return -1;
        }
        TimerHandler handler;
        if (_pool.Count > 0)
        {
            handler = _pool[_pool.Count - 1];
            _pool.Remove(handler);
        }
        else
        {
            handler = new TimerHandler();
        }
        handler.userFrame = useFrame;
        handler.repeat = repeat;
        handler.delay = delay;
        handler.method = method;
        handler.args = args;
        handler.exeTime = delay + (useFrame ? _currFrame : currentTime);
        _handlers.Add(handler);
        return method;
    }

    /// /// <summary>
    /// ��ʱִ��һ��(���ں���)
    /// </summary>
    /// <param name="delay">�ӳ�ʱ��(��λ����)</param>
    /// <param name="method">����ʱ�Ļص�����</param>
    /// <param name="args">�ص�����</param>
    public void doOnce(int delay, Handler method)
    {
        create(false, false, delay, method);
    }
    public void doOnce<T1>(int delay, Handler<T1> method, params object[] args)
    {
        create(false, false, delay, method, args);
    }
    public void doOnce<T1, T2>(int delay, Handler<T1, T2> method, params object[] args)
    {
        create(false, false, delay, method, args);
    }
    public void doOnce<T1, T2, T3>(int delay, Handler<T1, T2, T3> method, params object[] args)
    {
        create(false, false, delay, method, args);
    }

    /// /// <summary>
    /// ��ʱ�ظ�ִ��(���ں���)
    /// </summary>
    /// <param name="delay">�ӳ�ʱ��(��λ����)</param>
    /// <param name="method">����ʱ�Ļص�����</param>
    /// <param name="args">�ص�����</param>
    public void doLoop(int delay, Handler method)
    {
        create(false, true, delay, method);
    }
    public void doLoop<T1>(int delay, Handler<T1> method, params object[] args)
    {
        create(false, true, delay, method, args);
    }
    public void doLoop<T1, T2>(int delay, Handler<T1, T2> method, params object[] args)
    {
        create(false, true, delay, method, args);
    }
    public void doLoop<T1, T2, T3>(int delay, Handler<T1, T2, T3> method, params object[] args)
    {
        create(false, true, delay, method, args);
    }


    /// <summary>
    /// ��ʱִ��һ��(����֡��)
    /// </summary>
    /// <param name="delay">�ӳ�ʱ��(��λΪ֡)</param>
    /// <param name="method">����ʱ�Ļص�����</param>
    /// <param name="args">�ص�����</param>
    public void doFrameOnce(int delay, Handler method)
    {
        create(true, false, delay, method);
    }
    public void doFrameOnce<T1>(int delay, Handler<T1> method, params object[] args)
    {
        create(true, false, delay, method, args);
    }
    public void doFrameOnce<T1, T2>(int delay, Handler<T1, T2> method, params object[] args)
    {
        create(true, false, delay, method, args);
    }
    public void doFrameOnce<T1, T2, T3>(int delay, Handler<T1, T2, T3> method, params object[] args)
    {
        create(true, false, delay, method, args);
    }

    /// <summary>
    /// ��ʱ�ظ�ִ��(����֡��)
    /// </summary>
    /// <param name="delay">�ӳ�ʱ��(��λΪ֡)</param>
    /// <param name="method">����ʱ�Ļص�����</param>
    /// <param name="args">�ص�����</param>
    public void doFrameLoop(int delay, Handler method)
    {
        create(true, true, delay, method);
    }
    public void doFrameLoop<T1>(int delay, Handler<T1> method, params object[] args)
    {
        create(true, true, delay, method, args);
    }
    public void doFrameLoop<T1, T2>(int delay, Handler<T1, T2> method, params object[] args)
    {
        create(true, true, delay, method, args);
    }
    public void doFrameLoop<T1, T2, T3>(int delay, Handler<T1, T2, T3> method, params object[] args)
    {
        create(true, true, delay, method, args);
    }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    /// <param name="method">methodΪ�ص���������</param>
    public void clearTimer(Handler method)
    {
        clear(method);
    }
    public void clearTimer<T1>(Handler<T1> method)
    {
        clear(method);
    }
    public void clearTimer<T1, T2>(Handler<T1, T2> method)
    {
        clear(method);
    }
    public void clearTimer<T1, T2, T3>(Handler<T1, T2, T3> method)
    {
        clear(method);
    }

    private void clear(Delegate method)
    {
        TimerHandler handler = _handlers.FirstOrDefault(t => t.method == method);
        if (handler != null)
        {
            _handlers.Remove(handler);
            handler.clear();
            _pool.Add(handler);
        }
    }

    /// <summary>
    /// �������ж�ʱ��
    /// </summary>
    public void clearAllTimer()
    {
        foreach (TimerHandler handler in _handlers)
        {
            clear(handler.method);
            clearAllTimer();
            return;
        }
    }

    public static void RemoveTimerMgr(TimerMgr timerMgr)
    {
        timerList.Remove(timerMgr);
    }

    /// <summary>
    /// ��Ϸ����������ʱ�䣬����
    /// </summary>
    public long currentTime
    {
        get { return (long)(Time.time * 1000); }
    }

    /**��ʱ������*/

    private class TimerHandler
    {
        /**ִ�м��*/
        public int delay;
        /**�Ƿ��ظ�ִ��*/
        public bool repeat;
        /**�Ƿ���֡��*/
        public bool userFrame;

        /**ִ��ʱ��*/
        public long exeTime;

        /**������*/
        public Delegate method;

        /**����*/
        public object[] args;

        /**����*/

        public void clear()
        {
            method = null;
            args = null;
        }
    }
}
