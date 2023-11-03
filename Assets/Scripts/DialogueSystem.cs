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
        #region ����ģʽ
        private static DialogueSystem instance = null;
        public static DialogueSystem Instance
        {
            get { return instance; }
        }

        #endregion

        #region ����
        /// <summary>
        /// �Ի������������л��ֵ䣬keyΪ�������ı�ţ�valueΪ�������ű�
        /// </summary>
        public SerializableDictionary<int, DialogueTrigger> triggerDic = new SerializableDictionary<int, DialogueTrigger>();
        /// <summary>
        /// ��ǰ���ڴ����ĶԻ�������
        /// </summary>
        public DialogueTrigger dialogueTrigger = null;
        /// <summary>
        /// �Ի��ı���0Ϊ�԰����֣�1Ϊ˵������
        /// </summary>
        public TMP_Text[] dialogTexts = null;
        /// <summary>
        /// �Ի��򱳾�
        /// </summary>
        private Image background = null; 
        /// <summary>
        /// ���ű�������
        /// </summary>
        public Sprite[] backgroundSprite = null;
        /// <summary>
        /// ͷ�����
        /// </summary>
        private Image avatar = null;
        /// <summary>
        /// �����ı�
        /// </summary>
        private TMP_Text nameText = null;
        /// <summary>
        /// �Ի��ı�
        /// </summary>
        private TMP_Text dialogText = null;
        /// <summary>
        /// �Ի�ϵͳ������ڵ�
        /// </summary>
        private GameObject dialogueNode = null; 
        /// <summary>
        /// keyΪ��ɫ���֣�valueΪ��ɫͷ��Ŀ����л��ֵ�
        /// </summary>
        public SerializableDictionary<string, Sprite> imageDic = new SerializableDictionary<string, Sprite>();
        /// <summary>
        /// ��ǰ���ı� ID
        /// </summary>
        private int dialogIndex = 0;
        /// <summary>
        /// ����ѡ��ѡ��
        /// </summary>
        private bool isChoosing = false;
        /// <summary>
        /// ���ڽ��жԻ�
        /// </summary>
        public bool inDialogue = false;
        /// <summary>
        /// ����ÿһ�жԻ��ı�������
        /// </summary>
        private string[] dialogRows = null;
        /// <summary>
        /// ÿһ�еĸ�������
        /// </summary>
        private string[] cells = null; 
        /// <summary>
        /// ѡ�ťԤ�Ƽ�
        /// </summary>
        public GameObject optionButton = null;
        /// <summary>
        /// ѡ�ť������
        /// </summary>
        private Transform buttonGroup = null; 

        /// <summary>
        /// �ı��Ƿ���ʾ���
        /// </summary>
        private bool textIsFinished = false;
        /// <summary>
        /// ÿ���ֵ���ʾ�ٶ�
        /// </summary>
        public float textSpeed = 0.05f;
        /// <summary>
        /// �Ի��ļ���ַ��resources����ĵ�ַ
        /// </summary>
        public string DialogFileAddress = null;

        #endregion


        #region Unity Methods

        private void Awake() // ��������ͷ���Ӧ
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
        /// ��ʼ���Ի�ϵͳ
        /// </summary>
        private void InitDialogueSystem()
        {

            //��ȡ���
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
        /// ��ʾ�ı���ͷ��
        /// </summary>
        /// <param name="name">��ɫ����</param>
        /// <param name="text">�Ի��ı�</param>
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
        /// ��ʼ�Ի�
        /// </summary>
        /// <param name="dialogueTrigger">�Ի��������ű�</param>
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
        /// �˳��Ի�
        /// </summary>
        public void ExitDialogue()
        {
            dialogueNode.SetActive(false);
            inDialogue = false;
        }
        /// <summary>
        /// �ѶԻ��ı��ָ�ɸ���
        /// </summary>
        /// <param name="textAsset">�Ի��ļ�</param>
        public void ReadText(TextAsset textAsset)
        {
            dialogRows = textAsset.text.Split('\n');
        }
        /// <summary>
        /// ��ʾ�Ի���
        /// </summary>
        public void ShowDialogRow()
        {
            // Debug.Log(dialogIndex);
            for (int i = 1; i < dialogRows.Length; i++)
            {
                cells = dialogRows[i].Split(','); // �ѶԻ��зָ�ɸ�������

                if (int.Parse(cells[1]) != dialogIndex)
                {
                    continue;
                }

                if (cells[0] == "End") // ����ǽ����ڵ�������Ի�
                {
                    ExitDialogue();

                    if (cells[6] != "") //���Ч����Ϊ�գ��򴥷���̬�¼�
                    {
                        string[] effects = cells[6].Split('/'); // ��Ч����Ž��зָ�
                        foreach (string effect in effects)
                        {
                            DialogEffect(int.Parse(effect));
                        }
                    }

                    break;
                }

                if (cells[6] != "") //���Ч����Ϊ�գ��򴥷���̬�¼�
                {
                    string[] effects = cells[6].Split('/'); // ��Ч����Ž��зָ�
                    foreach (string effect in effects)
                    {
                        DialogEffect(int.Parse(effect));
                    }
                }

                if (cells[0] == "#") //�������ͨ�Ի��� ID �����ڽ��еĶԻ� ID ����ʾ
                {
                    UpdateText(cells[2], cells[3]);
                    Debug.Log("��ǰId" + dialogIndex);
                    dialogIndex = int.Parse(cells[4]); // ��ת��һ���Ի�
                    Debug.Log("��תId" + dialogIndex);
                    break;
                }
                else if (cells[0] == "&") // �����ѡ��Ի�����ʾ��ť
                {
                    UpdateText(cells[2], "");

                    GenerateOption(i);
                    break;
                }
            }
        }
        /// <summary>
        /// ������һ�жԻ�
        /// </summary>
        public void ContinueDialog() 
        {
            ShowDialogRow();
        }
        /// <summary>
        /// ���ɰ�ť����Ӱ�ť�¼�
        /// </summary>
        /// <param name="index"></param>
        public void GenerateOption(int index)
        {
            isChoosing = true;
            string[] cells = dialogRows[index].Split(',');
            string[] conditions = cells[5].Split('/'); // ��������Ž��зָ�
            if (cells[0] == "&")
            {
                GameObject button = Instantiate(optionButton, buttonGroup);
                button.GetComponentInChildren<TMP_Text>().text = cells[3];

                // ����ť����¼�
                button.GetComponent<Button>().onClick.AddListener
                (
                    () => { OnOptionClick(int.Parse(cells[4])); }
                );

                if (cells[5] != "") //���������������ж��Ƿ����ð�ť
                {
                    foreach (string condition in conditions) //�����һ������������ͽ��ð�ť
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
        /// ��Ӱ�ť�¼�
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
        /// ������̬�¼�
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
        /// ���ֻ�Ч��
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerator DisplayDialogue(string text)
        {
            // Debug.Log("��ʼ����");
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
        /// �����⣬���Էŵ�ͳһ��������ű���
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
