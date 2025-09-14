using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChatManager : MonoBehaviour
{
    [Header("UI�Q��")]
    public TMP_InputField messageInputField;
    public Button sendButton;
    public Transform chatContent;
    public GameObject chatBubblePrefab;

    [Header("�X�N���[���ݒ�")]
    public ScrollRect chatScrollRect;

    [Header("�����o���摜")]
    public Sprite playerBubbleSprite;
    public Sprite otherBubbleSprite;

    [Header("���b�Z�[�W�X�^�C��")]
    public Color playerTextColor = Color.white;
    public Color otherTextColor = Color.black;
    public Color playerBackgroundColor = new Color(0.3f, 0.6f, 1f, 1f);  // �F
    public Color otherBackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f); // �O���[

    [Header("�e�X�g�p")]
    public Button switchSenderButton;

    [Header("�^�C�s���O�ݒ�")]
    public float typingSpeed = 0.03f;

    private bool isPlayerMessage = true;

    void Start()
    {
        // ��{�I�Ȑݒ�
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);

        if (messageInputField != null)
            messageInputField.onSubmit.AddListener(OnEnterPressed);

        if (switchSenderButton != null)
            switchSenderButton.onClick.AddListener(SwitchSender);

        // �X�N���[���r���[�������擾
        if (chatScrollRect == null)
            chatScrollRect = GetComponentInChildren<ScrollRect>();

        Debug.Log("�`���b�g�V�X�e������������");
    }

    public void SendMessage()
    {
        if (messageInputField == null) return;

        string message = messageInputField.text.Trim();

        if (string.IsNullOrEmpty(message)) return;

        CreateChatBubble(message, isPlayerMessage);

        messageInputField.text = "";
        messageInputField.ActivateInputField();
    }

    void OnEnterPressed(string message)
    {
        SendMessage();
    }

    public void SwitchSender()
    {
        isPlayerMessage = !isPlayerMessage;

        if (switchSenderButton != null)
        {
            TMP_Text buttonText = switchSenderButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = isPlayerMessage ? "����ɐؑ�" : "�����ɐؑ�";
        }

        string currentSender = isPlayerMessage ? "����" : "����";
        Debug.Log("���M�҂�" + currentSender + "�ɕύX���܂���");
    }

    void CreateChatBubble(string message, bool isPlayer)
    {
        if (chatBubblePrefab == null || chatContent == null) return;

        GameObject newBubble = Instantiate(chatBubblePrefab, chatContent);

        // ���b�Z�[�W�e�L�X�g��ݒ�
        TMP_Text messageText = newBubble.GetComponentInChildren<TMP_Text>();
        if (messageText != null)
        {
            StartCoroutine(TypeMessage(messageText, message));
        }

        // �X�^�C����ݒ�
        SetMessageStyle(newBubble, isPlayer);

        // �X�N���[��
        StartCoroutine(ScrollToBottom());
    }

    void SetMessageStyle(GameObject bubble, bool isPlayer)
    {
        if (bubble == null) return;

        // �w�i�摜��ݒ�
        Image backgroundImage = GetBackgroundImage(bubble);
        if (backgroundImage != null)
        {
            SetBackgroundImage(backgroundImage, isPlayer);
        }

        // ���C�A�E�g��ݒ�
        SetBubbleLayout(bubble, isPlayer);

        // �e�L�X�g�F��ݒ�
        SetTextColor(bubble, isPlayer);
    }

    Image GetBackgroundImage(GameObject bubble)
    {
        // �܂����O�ŒT��
        Transform background = bubble.transform.Find("Background");
        if (background != null)
        {
            Image img = background.GetComponent<Image>();
            if (img != null) return img;
        }

        // ������Ȃ���΍ŏ���Image���擾
        Image[] images = bubble.GetComponentsInChildren<Image>();
        if (images.Length > 0)
            return images[0];

        return null;
    }

    void SetBackgroundImage(Image backgroundImage, bool isPlayer)
    {
        if (backgroundImage == null) return;

        // 9-slice���g��Ȃ��V���v���ȕ��@
        if (isPlayer)
        {
            if (playerBubbleSprite != null)
            {
                backgroundImage.sprite = playerBubbleSprite;
                backgroundImage.color = Color.white;
                backgroundImage.type = Image.Type.Simple;  // Simple�ɕύX
                backgroundImage.preserveAspect = true;      // �A�X�y�N�g����ێ�
                Debug.Log("�΂̐����o���摜��ݒ�iSimple�j");
            }
            else
            {
                backgroundImage.sprite = null;
                backgroundImage.color = Color.green;
            }
        }
        else
        {
            if (otherBubbleSprite != null)
            {
                backgroundImage.sprite = otherBubbleSprite;
                backgroundImage.color = Color.white;
                backgroundImage.type = Image.Type.Simple;  // Simple�ɕύX
                backgroundImage.preserveAspect = true;      // �A�X�y�N�g����ێ�
                Debug.Log("���̐����o���摜��ݒ�iSimple�j");
            }
            else
            {
                backgroundImage.sprite = null;
                backgroundImage.color = Color.white;
            }
        }
    }

    void SetBubbleLayout(GameObject bubble, bool isPlayer)
    {
        HorizontalLayoutGroup layoutGroup = bubble.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null) return;

        if (isPlayer)
        {
            // �����̃��b�Z�[�W�F�E��
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            layoutGroup.padding.left = 50;
            layoutGroup.padding.right = 10;
        }
        else
        {
            // ����̃��b�Z�[�W�F����
            layoutGroup.childAlignment = TextAnchor.MiddleLeft;
            layoutGroup.padding.left = 10;
            layoutGroup.padding.right = 50;
        }

        layoutGroup.padding.top = 5;
        layoutGroup.padding.bottom = 5;
    }

    void SetTextColor(GameObject bubble, bool isPlayer)
    {
        TMP_Text messageText = bubble.GetComponentInChildren<TMP_Text>();
        if (messageText != null)
        {
            messageText.color = isPlayer ? playerTextColor : otherTextColor;
        }
    }

    IEnumerator TypeMessage(TMP_Text textComponent, string message)
    {
        if (textComponent == null) yield break;

        textComponent.text = "";

        for (int i = 0; i < message.Length; i++)
        {
            textComponent.text += message[i];
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator ScrollToBottom()
    {
        yield return null;

        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
