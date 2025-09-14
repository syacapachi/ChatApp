using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChatManager : MonoBehaviour
{
    [Header("UI参照")]
    public TMP_InputField messageInputField;
    public Button sendButton;
    public Transform chatContent;
    public GameObject chatBubblePrefab;

    [Header("スクロール設定")]
    public ScrollRect chatScrollRect;

    [Header("吹き出し画像")]
    public Sprite playerBubbleSprite;
    public Sprite otherBubbleSprite;

    [Header("メッセージスタイル")]
    public Color playerTextColor = Color.white;
    public Color otherTextColor = Color.black;
    public Color playerBackgroundColor = new Color(0.3f, 0.6f, 1f, 1f);  // 青色
    public Color otherBackgroundColor = new Color(0.9f, 0.9f, 0.9f, 1f); // グレー

    [Header("テスト用")]
    public Button switchSenderButton;

    [Header("タイピング設定")]
    public float typingSpeed = 0.03f;

    private bool isPlayerMessage = true;

    void Start()
    {
        // 基本的な設定
        if (sendButton != null)
            sendButton.onClick.AddListener(SendMessage);

        if (messageInputField != null)
            messageInputField.onSubmit.AddListener(OnEnterPressed);

        if (switchSenderButton != null)
            switchSenderButton.onClick.AddListener(SwitchSender);

        // スクロールビューを自動取得
        if (chatScrollRect == null)
            chatScrollRect = GetComponentInChildren<ScrollRect>();

        Debug.Log("チャットシステム初期化完了");
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
                buttonText.text = isPlayerMessage ? "相手に切替" : "自分に切替";
        }

        string currentSender = isPlayerMessage ? "自分" : "相手";
        Debug.Log("送信者を" + currentSender + "に変更しました");
    }

    void CreateChatBubble(string message, bool isPlayer)
    {
        if (chatBubblePrefab == null || chatContent == null) return;

        GameObject newBubble = Instantiate(chatBubblePrefab, chatContent);

        // メッセージテキストを設定
        TMP_Text messageText = newBubble.GetComponentInChildren<TMP_Text>();
        if (messageText != null)
        {
            StartCoroutine(TypeMessage(messageText, message));
        }

        // スタイルを設定
        SetMessageStyle(newBubble, isPlayer);

        // スクロール
        StartCoroutine(ScrollToBottom());
    }

    void SetMessageStyle(GameObject bubble, bool isPlayer)
    {
        if (bubble == null) return;

        // 背景画像を設定
        Image backgroundImage = GetBackgroundImage(bubble);
        if (backgroundImage != null)
        {
            SetBackgroundImage(backgroundImage, isPlayer);
        }

        // レイアウトを設定
        SetBubbleLayout(bubble, isPlayer);

        // テキスト色を設定
        SetTextColor(bubble, isPlayer);
    }

    Image GetBackgroundImage(GameObject bubble)
    {
        // まず名前で探す
        Transform background = bubble.transform.Find("Background");
        if (background != null)
        {
            Image img = background.GetComponent<Image>();
            if (img != null) return img;
        }

        // 見つからなければ最初のImageを取得
        Image[] images = bubble.GetComponentsInChildren<Image>();
        if (images.Length > 0)
            return images[0];

        return null;
    }

    void SetBackgroundImage(Image backgroundImage, bool isPlayer)
    {
        if (backgroundImage == null) return;

        // 9-sliceを使わないシンプルな方法
        if (isPlayer)
        {
            if (playerBubbleSprite != null)
            {
                backgroundImage.sprite = playerBubbleSprite;
                backgroundImage.color = Color.white;
                backgroundImage.type = Image.Type.Simple;  // Simpleに変更
                backgroundImage.preserveAspect = true;      // アスペクト比を維持
                Debug.Log("緑の吹き出し画像を設定（Simple）");
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
                backgroundImage.type = Image.Type.Simple;  // Simpleに変更
                backgroundImage.preserveAspect = true;      // アスペクト比を維持
                Debug.Log("白の吹き出し画像を設定（Simple）");
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
            // 自分のメッセージ：右寄せ
            layoutGroup.childAlignment = TextAnchor.MiddleRight;
            layoutGroup.padding.left = 50;
            layoutGroup.padding.right = 10;
        }
        else
        {
            // 相手のメッセージ：左寄せ
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
