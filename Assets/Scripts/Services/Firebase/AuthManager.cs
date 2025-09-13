using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;

public partial class AuthManager : AuthManagerBase
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;

    // ���̂���������
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthManager();
        Debug.Log("AuthManager(Firebase) ����������");
    }

    private AuthManager()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public override string CurrentUserId => auth.CurrentUser?.UserId;
    public override string CurrentUserName => auth.CurrentUser?.DisplayName;

    public override void SignUp(string email, string password, string username, Action<bool, string> callback)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback(false, task.Exception?.Message ?? "�G���[");
                return;
            }

            currentUser = task.Result.User;
            if (string.IsNullOrEmpty(username))
                username = currentUser.UserId;

            currentUser.UpdateUserProfileAsync(new UserProfile { DisplayName = username });

            // Firestore �ɕۑ�
            var db = FirebaseFirestore.DefaultInstance;
            db.Collection("users").Document(CurrentUserId).SetAsync(new Dictionary<string, object> {
                { "username", username },
                { "email", email }
            });

            callback(true, "�T�C���A�b�v����");
        });
    }

    public override void SignIn(string email, string password, Action<bool, string> callback)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback(false, task.Exception?.Message ?? "�G���[");
                return;
            }
            currentUser = task.Result.User;
            callback(true, "���O�C������");
        });
    }

    public override void SignOut()
    {
        auth.SignOut();
        currentUser = null;
    }
}
