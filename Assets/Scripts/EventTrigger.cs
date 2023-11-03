using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    /// <summary>
    /// �¼�����������������Ի������¼�
    /// </summary>
    public class EventTrigger : MonoBehaviour
    {
        /// <summary>
        /// �������¼��ı��
        /// </summary>
        public int eventIndex; 
        /// <summary>
        /// ������ǰ��������ţ�Ĭ��Ϊ 0 ��������
        /// </summary>
        public int conditionIndex; 
        /// <summary>
        /// �Ƿ��ڽ��볡��/���� ʱ���Զ������¼�
        /// </summary>
        public bool autoEvent;


        private void Awake()
        {

        }

        private void OnEnable() // ���� autoEvent �����봥�����ļ����������Զ������¼�
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
