using System.Collections;
using UnityEngine;

/// <summary>
/// �㿪������ʱ�䲻����
/// </summary>
public class LongTimeNoOperationOneTime : MonoBehaviour
{   
    private static LongTimeNoOperationOneTime _instance;

    public static LongTimeNoOperationOneTime Instance
    {
        get
        {
            if (_instance == null)
            {
                LongTimeNoOperationOneTime ins = FindObjectOfType<LongTimeNoOperationOneTime>();
                if (ins == null)
                {
                    Debug.LogError("������û��LongTimeNoOperationOneTime����������");
                }
                else
                {
                    _instance = ins;
                }
            }
            return _instance;
        }
        
    }


    private bool check = false;//�Ƿ�ʼ���
    public float lasterTime;//�ϴε�ʱ��
    public float nowTime;//��ǰʱ��

    public void Update()
    {
        if (check && Common.backtime > 0) CheckOperate();
    }

    /// <summary>
    /// �����޲������
    /// </summary>
    public void CheckOperate()
    {
        //��ǰʱ��
        nowTime = Time.time;

        //����в���������ϴβ���ʱ��Ϊ��ʱ
        if (Application.isEditor)//�ڱ༭��������
        {
            //�����������/�в���������´���ʱ��
            if (Input.GetMouseButtonDown(0)) lasterTime = nowTime;
        }
        //�Ǳ༭�������£���������
        //Input.touchCount��pc��û�ã�ֻ���ƶ�����Ч
        //Application.isMobilePlatform��pc���ƶ��˶���Ч
        else if (Application.isMobilePlatform)
        {
            //����Ļ��ָ �Ӵ�
            if (Input.touchCount > 0) lasterTime = nowTime;//���´���ʱ��
        }

        //�ж��޲���ʱ���Ƿ�ﵽָ��ʱ�������ﵽָ��ʱ���޲�������ִ��TakeOperate
        float offsetTime = Mathf.Abs(nowTime - lasterTime);
        if (offsetTime > Common.backtime) TakeOperate();

    }

    /// <summary>
    /// ����ʱ���޲���ʱִ���������
    /// </summary>
    public void TakeOperate()
    {
        Debug.Log("ִ��");
        check = false;
    }
    public void StartTime()
    {
        check = true;
        lasterTime = Time.time;
    }
}