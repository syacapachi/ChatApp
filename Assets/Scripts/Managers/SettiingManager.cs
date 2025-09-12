using UnityEngine;

public class SettiingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SettingsService settingsService;
    void Start()
    {
        settingsService = new FirebaseAuthService();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDeleteUserButton()
    {
        settingsService.CurrentUser();
        settingsService.DeleteUserData((success, message) =>
        {
            Debug.Log(message);
            if (success)
            {
                Debug.Log("Delete Success!");
                settingsService.SignOut();
            }
        });
        
        
    }
}
