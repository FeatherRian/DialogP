
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region  Interface
public interface IEventList
{
    public bool isStaticEvent(int index);
    public bool ActiveEvent(int index);//通过索引值触发动态事件的功能函数,成功调用返回true
}
#endregion

public class EventSystem : MonoBehaviour, IEventList
{
    #region  Properties
    private static EventSystem instance = null;
    public static EventSystem Instance { get { return instance; } } // 单例模式 
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
    /// 初始化事件系统
    /// </summary>
    public void InitEventSystem()
    {
        staticEventList = new int[100];
        //手动设置开关初始状态
        staticEventList[16] = 1;
        staticEventList[17] = 1;
        staticEventList[18] = 1;
        staticEventList[19] = 1;
        staticEventList[20] = 1;
    }
    /// <summary>
    /// 改变静态事件
    /// </summary>
    /// <param name="index">事件索引</param>
    /// <param name="active">激活或者关闭</param>
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
    /// 查询静态事件
    /// </summary>
    /// <param name="index">事件索引</param>
    /// <returns>静态事件状态</returns>
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
    /// 进行动态事件
    /// </summary>
    /// <param name="index">事件索引</param>
    /// <returns></returns>
    public bool ActiveEvent(int index) //手动填入动态事件
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
