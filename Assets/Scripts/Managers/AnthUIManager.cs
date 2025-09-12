using Firebase.Auth;
using TMPro;
using UnityEngine;

public class AnthUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI emailTextMeshPro;
    [SerializeField] TextMeshProUGUI passwaordTextMeshPro;
    [SerializeField] TextMeshProUGUI userNameTextMeshPro;
    private IAuthService auth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        auth = new FirebaseAuthService();
    }
    private void Start()
    {
        
    }
    public void OnSignUpButtonClick()
    {
        if (emailTextMeshPro == null || passwaordTextMeshPro == null) {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return; 
        }
        string email = emailTextMeshPro.text;
        string pass = passwaordTextMeshPro.text;
        string userName = userNameTextMeshPro.text;
        if (email != "" && pass != "")
        {
           
            auth.SignUp(email, pass,userName, (success, message) =>
            {
                Debug.Log(message);
                if (success)
                {
                    Debug.Log("SiginUp Success!");
                    Debug.Log($"UserID:{auth.CurrentUserId}");
                }
            });
            
            
        }
        
    }
    public void OnSignInButtonClick()
    {
        if (emailTextMeshPro == null || passwaordTextMeshPro == null)
        {
            Debug.LogError("NullReferenceError:TextMeshProUGUI is not defined");
            return;
        }
        string email = emailTextMeshPro.text;
        string pass = passwaordTextMeshPro.text;
        if (email != "" && pass != "")
        {
            auth.SignIn(email, pass, (success, message) =>
            {
                Debug.Log(message);
                if (success)
                {
                    Debug.Log("SiginIn Success!");
                    Debug.Log($"UserID:{auth.CurrentUserId}");
                }
            });
            
        }
    }
    public void OnSignOutButtonClick()
    {
        auth.SignOut() ;
        Debug.Log("SiginOut Success!");
    }
    
}
