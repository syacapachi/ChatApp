using TMPro;
using UnityEngine;

public class UserHomeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IAuthService auth;
    void Start()
    {
        auth = new FirebaseAuthService();
        userNameGUI.text = auth.UserName;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
