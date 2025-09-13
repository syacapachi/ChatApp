using TMPro;
using UnityEngine;

public class UserHomeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userNameGUI.text = AuthManagerBase.Instance.CurrentUserName;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSignOutButtonClick()
    {
        AuthManagerBase.Instance.SignOut();
        Debug.Log("SiginOut Success!");
    }
}
