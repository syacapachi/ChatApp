<<<<<<< Updated upstream
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ChatManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button sendButton;
    [SerializeField] Transform content; // ScrollView ‚Ì Content
    [SerializeField] GameObject messagePrefab;

    private FirebaseFirestore db;
    private FirebaseUser user;
    private ListenerRegistration listener;

    [Header("Room Settings")]
    public string roomId; // ‘I‘ğ‚³‚ê‚½ƒ`ƒƒƒbƒgƒ‹[ƒ€‚ÌID

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        sendButton.onClick.AddListener(SendMessage);

        // —š—ğ‚ğƒŠƒAƒ‹ƒ^ƒCƒ€‚ÅóM
        ListenMessages();
    }

    void OnDestroy()
    {
        listener?.Stop();
    }

    void SendMessage()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;

        var msg = new Dictionary<string, object>
        {
            { "senderId", user.UserId },
            { "message", inputField.text },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        };

        db.Collection("chatRooms").Document(roomId)
          .Collection("messages").AddAsync(msg);

        inputField.text = ""; // “ü—Í—“‚ğƒNƒŠƒA
    }

    void ListenMessages()
    {
        listener = db.Collection("chatRooms").Document(roomId)
            .Collection("messages")
            .OrderBy("timestamp")
            .Listen(snapshot =>
            {
                // ŒÃ‚¢ƒƒbƒZ[ƒW‚ğˆê“xƒNƒŠƒAiŠÈˆÕÀ‘•j
                foreach (Transform child in content)
                {
                    Destroy(child.gameObject);
                }

                // ÅV‚ÌƒƒbƒZ[ƒW‚ğUI‚É”½‰f
                foreach (var doc in snapshot.Documents)
                {
                    string senderId = doc.GetValue<string>("senderId");
                    string text = doc.GetValue<string>("message");

                    GameObject msgObj = Instantiate(messagePrefab, content);
                    var msgText = msgObj.GetComponentInChildren<TMP_Text>();
                    msgText.text = text;

                    // ©•ª‚ÌƒƒbƒZ[ƒW‚ÍF‚âˆÊ’u‚ğ•Ï‚¦‚é
                    if (senderId == user.UserId)
                    {
                        msgText.color = Color.green;
                        msgObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(200, 0);
                    }
                    else
                    {
                        msgText.color = Color.white;
=======
ï»¿using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatManager : ChatManagerBase
{
    private FirebaseFirestore db;
    private FirebaseUser user;
    //ãƒ‡ãƒ¼ã‚¿æ›´æ–°ã‚’è¦‹ã‚‹ã‚„ã¤
    private ListenerRegistration listener;
    private string roomId;
    public override event Action<string,string,string> OnMessageReceived;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new ChatManager();
        Debug.Log("ChatManager(Firebase) åˆæœŸåŒ–å®Œäº†");
    }
    private ChatManager()
    {
        db = FirebaseFirestore.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }
   
    public async override void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        try
        {
            var msg = new Dictionary<string, object>
            {
                { "senderId", user.UserId },
                { "senderName",user.DisplayName},
                { "message", message },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };

            await db.Collection("chatRooms").Document(roomId)
              .Collection("messages").AddAsync(msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        

    }
    public override void StartListenMessages(string _roomId)
    {
        roomId = _roomId;
        listener = db.Collection("chatRooms").Document(roomId)
            .Collection("messages")
            .OrderBy("timestamp")
            .Listen(async snapshot =>
            {
                foreach (var docChange in snapshot.GetChanges())
                {
                    if (docChange.ChangeType == DocumentChange.Type.Added)
                    {
                        string userID = docChange.Document.GetValue<string>("senderId");
                        string username = await GetUserName(userID);
                        string msg = docChange.Document.GetValue<string>("message");
                        string timestamp = docChange.Document.GetValue<string>("timestamp");
                        Debug.Log($"[Firestore] å—ä¿¡: {msg}");
                        OnMessageReceived?.Invoke(msg,username,timestamp); // UIå´ã¸é€šçŸ¥
>>>>>>> Stashed changes
                    }
                }
            });
    }
<<<<<<< Updated upstream
=======
    public override void StopLister()
    {
        listener?.Dispose();
    }
    private async Task<string> GetUserName(string userId)
    {
        DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();
        if (snapshot.Exists && snapshot.ContainsField("username"))
        {
            return snapshot.GetValue<string>("username");
        }
        return "Unknown";
    }
>>>>>>> Stashed changes
}
