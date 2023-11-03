using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    /// <summary>
    /// 事件触发器，用于脱离对话触发事件
    /// </summary>
    public class EventTrigger : MonoBehaviour
    {
        /// <summary>
        /// 触发的事件的编号
        /// </summary>
        public int eventIndex; 
        /// <summary>
        /// 触发的前置条件编号，默认为 0 即无条件
        /// </summary>
        public int conditionIndex; 
        /// <summary>
        /// 是否在进入场景/房间 时就自动触发事件
        /// </summary>
        public bool autoEvent;


        private void Awake()
        {

        }

        private void OnEnable() // 依靠 autoEvent 变量与触发器的激活来开启自动触发事件
        {
            if (autoEvent && EventSystem.Instance.isStaticEvent(conditionIndex))
            {
                StartEvent();
            }
        }

        private void Update()
        {

        }


        #region TriggerEvent

        public void StartEvent()
        {
            if (eventIndex > 0)
            {
                EventSystem.Instance.ActiveEvent(eventIndex);
            }
            if (eventIndex < 0)
            {
                EventSystem.Instance.changeStaticEvent(eventIndex, true);
            }
        }

        public void OnClickEvent()
        {
            if (EventSystem.Instance.isStaticEvent(conditionIndex))
            {
                StartEvent();
            }
        }

        #endregion
    }
}
