using System;
using UnityEngine;
using Firebase.Auth;

public partial class AdministerManager : AdministerManagerBase
{
    FirebaseAuth auth;
    FirebaseUser currentuser;
    // ���̂���������
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    
    private static void Init()
    {
        Instance = new AdministerManager();
        Debug.Log("AdministerManager(Firebase) ����������");
    }
    private AdministerManager() 
    {
        auth = FirebaseAuth.DefaultInstance;
        currentuser = auth.CurrentUser;
    }
    public override void DeleteUser(Action<bool, string> callback)
    {
        currentuser.DeleteAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            if (task.IsCanceled)
            {
                callback(false, task.Exception.ToString());
                return;
            }
            callback(true, "���[�U�[�f�[�^�폜����");
        });
    }
}
