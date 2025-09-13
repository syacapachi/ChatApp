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

    // 実体を差し込む
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthManager();
        Debug.Log("AuthManager(Firebase) 初期化完了");
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
                callback(false, task.Exception?.Message ?? "エラー");
                return;
            }

            currentUser = task.Result.User;
            if (string.IsNullOrEmpty(username))
                username = currentUser.UserId;

            currentUser.UpdateUserProfileAsync(new UserProfile { DisplayName = username });

            // Firestore に保存
            var db = FirebaseFirestore.DefaultInstance;
            db.Collection("users").Document(CurrentUserId).SetAsync(new Dictionary<string, object> {
                { "username", username },
                { "email", email }
            });

            callback(true, "サインアップ成功");
        });
    }

    public override void SignIn(string email, string password, Action<bool, string> callback)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                callback(false, task.Exception?.Message ?? "エラー");
                return;
            }
            currentUser = task.Result.User;
            callback(true, "ログイン成功");
        });
    }

    public override void SignOut()
    {
        auth.SignOut();
        currentUser = null;
    }
}
