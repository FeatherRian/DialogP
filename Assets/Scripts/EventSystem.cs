
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region  Interface
public interface IEventList
{
    public bool isStaticEvent(int index);
    public bool ActiveEvent(int index);//ͨ������ֵ������̬�¼��Ĺ��ܺ���,�ɹ����÷���true
}
#endregion

public class EventSystem : MonoBehaviour, IEventList
{
    #region  Properties
    private static EventSystem instance = null;
    public static EventSystem Instance { get { return instance; } } // ����ģʽ 
    public int[] staticEventList = new int[100];
    #endregion

    #region Unity Methods

    private void Awake() 
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitEventSystem();
    }

    #endregion

    #region  EventSystem
    /// <summary>
    /// ��ʼ���¼�ϵͳ
    /// </summary>
    public void InitEventSystem()
    {
        staticEventList = new int[100];
        //�ֶ����ÿ��س�ʼ״̬
        staticEventList[16] = 1;
        staticEventList[17] = 1;
        staticEventList[18] = 1;
        staticEventList[19] = 1;
        staticEventList[20] = 1;
    }
    /// <summary>
    /// �ı侲̬�¼�
    /// </summary>
    /// <param name="index">�¼�����</param>
    /// <param name="active">������߹ر�</param>
    public void changeStaticEvent(int index, bool active) 
    {
        if (active)
        {
            staticEventList[index] = 1;
        }
        else
        {
            staticEventList[index] = 0;
        }

    }
    /// <summary>
    /// ��ѯ��̬�¼�
    /// </summary>
    /// <param name="index">�¼�����</param>
    /// <returns>��̬�¼�״̬</returns>
    public bool isStaticEvent(int index) 
    {
        if (index == 0)
        {
            return true;
        }
        // Debug.Log(staticEventList[index]);
        if (staticEventList[index] == 0)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// ���ж�̬�¼�
    /// </summary>
    /// <param name="index">�¼�����</param>
    /// <returns></returns>
    public bool ActiveEvent(int index) //�ֶ����붯̬�¼�
    {
        switch (index)
        {
            case 1: Test1(); break;
            case 2: Test2(); break;
            default: return false;
        }
        return true;
    }
    #endregion

    #region  ActiveEvents

    private void Test1()
    {
        Debug.Log("Test1");
    }

    private void Test2()
    {
        Debug.Log("Test2");
    }

    #endregion

}
