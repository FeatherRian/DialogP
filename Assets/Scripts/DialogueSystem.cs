using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Dialog.Dictionary;

namespace Dialog
{
    public class DialogueSystem : MonoBehaviour
    {
        #region 单例模式
        private static DialogueSystem instance = null;
        public static DialogueSystem Instance
        {
            get { return instance; }
        }

        #endregion

        #region 属性
        /// <summary>
        /// 对话触发器的序列化字典，key为触发器的编号，value为触发器脚本
        /// </summary>
        public SerializableDictionary<int, DialogueTrigger> triggerDic = new SerializableDictionary<int, DialogueTrigger>();
        /// <summary>
        /// 当前正在触发的对话触发器
        /// </summary>
        public DialogueTrigger dialogueTrigger = null;
        /// <summary>
        /// 对话文本，0为旁白文字，1为说话文字
        /// </summary>
        public TMP_Text[] dialogTexts = null;
        /// <summary>
        /// 对话框背景
        /// </summary>
        private Image background = null; 
        /// <summary>
        /// 两张背景画面
        /// </summary>
        public Sprite[] backgroundSprite = null;
        /// <summary>
        /// 头像组件
        /// </summary>
        private Image avatar = null;
        /// <summary>
        /// 名字文本
        /// </summary>
        private TMP_Text nameText = null;
        /// <summary>
        /// 对话文本
        /// </summary>
        private TMP_Text dialogText = null;
        /// <summary>
        /// 对话系统的整体节点
        /// </summary>
        private GameObject dialogueNode = null; 
        /// <summary>
        /// key为角色名字，value为角色头像的可序列化字典
        /// </summary>
        public SerializableDictionary<string, Sprite> imageDic = new SerializableDictionary<string, Sprite>();
        /// <summary>
        /// 当前的文本 ID
        /// </summary>
        private int dialogIndex = 0;
        /// <summary>
        /// 正在选择选项
        /// </summary>
        private bool isChoosing = false;
        /// <summary>
        /// 正在进行对话
        /// </summary>
        public bool inDialogue = false;
        /// <summary>
        /// 储存每一行对话文本的数组
        /// </summary>
        private string[] dialogRows = null;
        /// <summary>
        /// 每一行的各个部分
        /// </summary>
        private string[] cells = null; 
        /// <summary>
        /// 选项按钮预制件
        /// </summary>
        public GameObject optionButton = null;
        /// <summary>
        /// 选项按钮父物体
        /// </summary>
        private Transform buttonGroup = null; 

        /// <summary>
        /// 文本是否显示完毕
        /// </summary>
        private bool textIsFinished = false;
        /// <summary>
        /// 每个字的显示速度
        /// </summary>
        public float textSpeed = 0.05f;
        /// <summary>
        /// 对话文件地址，resources后面的地址
        /// </summary>
        public string DialogFileAddress = null;

        #endregion


        #region Unity Methods

        private void Awake() // 把名字与头像对应
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            InitDialogueSystem();
        }

        private void Update()
        {
            InputDetect();
        }

        #endregion


        #region Dialogue
        /// <summary>
        /// 初始化对话系统
        /// </summary>
        private void InitDialogueSystem()
        {

            //获取组件
            dialogueNode = GameObject.Find("DialogCanvas/Dialogue");
            avatar = dialogueNode.transform.Find("Avatar").GetComponent<Image>();
            nameText = dialogueNode.transform.Find("NameText").GetComponent<TMP_Text>();
            dialogTexts[0] = dialogueNode.transform.Find("DialogueText1").GetComponent<TMP_Text>();
            dialogTexts[1] = dialogueNode.transform.Find("DialogueText2").GetComponent<TMP_Text>();
            dialogText = dialogTexts[0];
            buttonGroup = dialogueNode.transform.Find("OptionsGroup");
            background = dialogueNode.transform.Find("Background").GetComponent<Image>();

            dialogueNode.SetActive(false);
        }
        /// <summary>
        /// 显示文本与头像
        /// </summary>
        /// <param name="name">角色名字</param>
        /// <param name="text">对话文本</param>
        public void UpdateText(string name, string text) 
        {
            nameText.text = name;
            if (name == "")
            {
                background.sprite = backgroundSprite[0];
                avatar.gameObject.SetActive(false);
                dialogText.gameObject.SetActive(false);
                dialogText = dialogTexts[0];
                dialogText.gameObject.SetActive(true);
            }
            else
            {
                background.sprite = backgroundSprite[1];
                avatar.gameObject.SetActive(true);
                avatar.sprite = imageDic[name];
                dialogText.gameObject.SetActive(false);
                dialogText = dialogTexts[1];
                dialogText.gameObject.SetActive(true);
            }
            dialogText.text = "";
            StartCoroutine("DisplayDialogue", text);

        }
        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="dialogueTrigger">对话触发器脚本</param>
        /// <returns></returns>
        public IEnumerator StartDialogue(DialogueTrigger dialogueTrigger)
        {
            this.dialogueTrigger = dialogueTrigger;
            dialogIndex = 0;
            dialogText.text = "";
            textIsFinished = false;
            dialogueNode.SetActive(true);
            ReadText(dialogueTrigger.dialogDataFile);
            ShowDialogRow();
            yield return new WaitForSeconds(0.1f);
            inDialogue = true;
        }
        /// <summary>
        /// 退出对话
        /// </summary>
        public void ExitDialogue()
        {
            dialogueNode.SetActive(false);
            inDialogue = false;
        }
        /// <summary>
        /// 把对话文本分割成各行
        /// </summary>
        /// <param name="textAsset">对话文件</param>
        public void ReadText(TextAsset textAsset)
        {
            dialogRows = textAsset.text.Split('\n');
        }
        /// <summary>
        /// 显示对话行
        /// </summary>
        public void ShowDialogRow()
        {
            // Debug.Log(dialogIndex);
            for (int i = 1; i < dialogRows.Length; i++)
            {
                cells = dialogRows[i].Split(','); // 把对话行分割成各个数据

                if (int.Parse(cells[1]) != dialogIndex)
                {
                    continue;
                }

                if (cells[0] == "End") // 如果是结束节点则结束对话
                {
                    ExitDialogue();

                    if (cells[6] != "") //如果效果不为空，则触发动态事件
                    {
                        string[] effects = cells[6].Split('/'); // 把效果编号进行分割
                        foreach (string effect in effects)
                        {
                            DialogEffect(int.Parse(effect));
                        }
                    }

                    break;
                }

                if (cells[6] != "") //如果效果不为空，则触发动态事件
                {
                    string[] effects = cells[6].Split('/'); // 把效果编号进行分割
                    foreach (string effect in effects)
                    {
                        DialogEffect(int.Parse(effect));
                    }
                }

                if (cells[0] == "#") //如果是普通对话且 ID 是正在进行的对话 ID 就显示
                {
                    UpdateText(cells[2], cells[3]);
                    Debug.Log("当前Id" + dialogIndex);
                    dialogIndex = int.Parse(cells[4]); // 跳转下一条对话
                    Debug.Log("跳转Id" + dialogIndex);
                    break;
                }
                else if (cells[0] == "&") // 如果是选择对话则显示按钮
                {
                    UpdateText(cells[2], "");

                    GenerateOption(i);
                    break;
                }
            }
        }
        /// <summary>
        /// 进行下一行对话
        /// </summary>
        public void ContinueDialog() 
        {
            ShowDialogRow();
        }
        /// <summary>
        /// 生成按钮并添加按钮事件
        /// </summary>
        /// <param name="index"></param>
        public void GenerateOption(int index)
        {
            isChoosing = true;
            string[] cells = dialogRows[index].Split(',');
            string[] conditions = cells[5].Split('/'); // 把条件编号进行分割
            if (cells[0] == "&")
            {
                GameObject button = Instantiate(optionButton, buttonGroup);
                button.GetComponentInChildren<TMP_Text>().text = cells[3];

                // 给按钮添加事件
                button.GetComponent<Button>().onClick.AddListener
                (
                    () => { OnOptionClick(int.Parse(cells[4])); }
                );

                if (cells[5] != "") //如果有条件则进入判断是否启用按钮
                {
                    foreach (string condition in conditions) //如果有一个条件不满足就禁用按钮
                    {
                        if (!EventSystem.Instance.isStaticEvent(int.Parse(condition)))
                        {
                            button.GetComponent<Button>().interactable = false;
                        }
                    }
                }
                GenerateOption(index + 1);
            }
        }

        /// <summary>
        /// 添加按钮事件
        /// </summary>
        /// <param name="id"></param>
        public void OnOptionClick(int id)
        {
            dialogIndex = id;
            Debug.Log(dialogIndex);

            for (int i = 0; i < buttonGroup.childCount; i++)
            {
                Destroy(buttonGroup.GetChild(i).gameObject);
            }
            ShowDialogRow();
            StartCoroutine("Chosen");
        }
        
        public IEnumerator Chosen()
        {
            yield return new WaitForSeconds(0.1f);
            isChoosing = false;
        }
        /// <summary>
        /// 触发动态事件
        /// </summary>
        /// <param name="index"></param>
        public void DialogEffect(int index)
        {
            if (index > 0)
            {
                EventSystem.Instance.ActiveEvent(index);
            }
            if (index < 0)
            {
                EventSystem.Instance.changeStaticEvent(index, true);
            }
        }
        /// <summary>
        /// 打字机效果
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerator DisplayDialogue(string text)
        {
            // Debug.Log("开始打字");
            textIsFinished = false;
            for (int i = 0; i < text.Length; i++)
            {
                dialogText.text += text[i];

                yield return new WaitForSeconds(textSpeed);
            }
            textIsFinished = true;
        }

        public IEnumerator FinishText()
        {
            yield return new WaitForSeconds(0.1f);
            textIsFinished = true;
        }
        /// <summary>
        /// 输入检测，可以放到统一的输入检测脚本中
        /// </summary>
        public void InputDetect()
        {
            if (!inDialogue)
            {
                return;
            }

            if ((Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0)) && !isChoosing)
            {
                if (textIsFinished)
                {
                    ContinueDialog();
                }
                else
                {
                    StopCoroutine("DisplayDialogue");
                    StartCoroutine("FinishText");
                    dialogText.text = cells[3];
                }
            }
        }

        public TextAsset GetTextAssets(int dialogIndex)
        {
            return Resources.Load<TextAsset>(DialogFileAddress + "/" + dialogIndex);
        }
        #endregion
    }
}
