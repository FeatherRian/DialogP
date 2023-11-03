using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dialog
{
    public class DialogueTrigger : MonoBehaviour
    {
        #region Properties

        public TextAsset dialogDataFile = null; // 对话文件，以 csv 形式保存]

        public int dialogIndex = 0;

        public bool autoEnterDialogue = false; // 是否在进入场景/房间 时就自动进行对话

        public bool DialogOnce = true; // 是否只能进行一次对话

        public bool canEnterDialog = true;

        #endregion

        #region Unity Methods

        public void Start()
        {
            DialogueSystem.Instance.triggerDic[dialogIndex] = this;
            dialogDataFile = DialogueSystem.Instance.GetTextAssets(dialogIndex);
        }

        public void OnEnable() // 依靠 autoEnterDialogue 变量与触发器的激活来开启自动进入对话
        {

        }

        public void Update() // 如果鼠标在触发器内且点击了，就进入对话
        {
            if ((DialogueSystem.Instance) && (!DialogueSystem.Instance.inDialogue))
            {
                AutoDialog();
            }
        }


        #endregion

        #region Trigger

        public void StartDialogue() //判断是否正在对话，如果没有正在对话则开始新的对话
        {
            if (DialogueSystem.Instance.inDialogue) { return; }
            if (!canEnterDialog) { return; }
            if (DialogOnce)
            {
                canEnterDialog = false;
            }
            DialogueSystem.Instance.StartCoroutine("StartDialogue", this);
        }

        public void AutoDialog()
        {
            if (autoEnterDialogue && canEnterDialog)
            {
                {
                    StartDialogue();
                    this.gameObject.SetActive(false);
                }
            }

        }
        #endregion
    }
}